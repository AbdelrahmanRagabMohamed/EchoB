using EchoB.Application.UseCases.Commands.Chat;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Chat
{
    public record UpdateMessageStatusCommand(string MessageId, MessageStatus Status) : IRequest;
    public class UpdateMessageStatusCommandHandler : IRequestHandler<UpdateMessageStatusCommand>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IPresenceService _presenceService;

        public UpdateMessageStatusCommandHandler(
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            IPresenceService presenceService)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _presenceService = presenceService;
        }

        public async Task Handle(UpdateMessageStatusCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the message
            var message = await _messageRepository.GetByIdAsync(request.MessageId);
            if (message == null)
            {
                throw new Exception($"Message with ID {request.MessageId} not found.");
            }

            // Validate status transition (e.g., can't go from Seen to Delivered)
            if (!IsValidStatusTransition(message.Status, request.Status))
            {
                throw new Exception($"Invalid status transition from {message.Status} to {request.Status}.");
            }

            // Update message status
            await _messageRepository.UpdateStatusAsync(request.MessageId, request.Status);

            // Update conversation's LastActivity
            var conversation = await _conversationRepository.GetByIdAsync(message.ConversationId.ToString());
            if (conversation != null)
            {
                conversation.LastActivity = DateTime.UtcNow;
                await _conversationRepository.CreateAsync(conversation); // Updates LastActivity
            }

            // Notify sender and recipient via Redis pub/sub
            var payload = new { MessageId = request.MessageId, Status = request.Status.ToString() };
            await _presenceService.PublishMessageAsync(message.SenderId.ToString(), "MessageStatusUpdated", payload);
            var recipientId = await GetRecipientIdAsync(message.ConversationId.ToString(), message.SenderId.ToString());
            if (recipientId != null)
            {
                await _presenceService.PublishMessageAsync(recipientId, "MessageStatusUpdated", payload);
            }
        }

        private bool IsValidStatusTransition(MessageStatus current, MessageStatus next)
        {
            return (current, next) switch
            {
                (MessageStatus.Sent, MessageStatus.Delivered) => true,
                (MessageStatus.Sent, MessageStatus.Read) => true,
                (MessageStatus.Delivered, MessageStatus.Read) => true,
                _ => false
            };
        }

        private async Task<string> GetRecipientIdAsync(string conversationId, string senderId)
        {
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            return conversation?.User1Id == Guid.Parse(senderId) ? conversation.User2Id.ToString() : conversation?.User1Id.ToString();
        }
    }
}
