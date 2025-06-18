using EchoB.Application.Common;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Chat;
using EchoB.Domain.Entities;
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
    public record SendMessageCommand(string UserId, string RecipientId, string Content) : IRequest<MessageDto>;
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IPresenceService _presenceService;
        private readonly IBlockedUserRepository _blockedUserRepository;

        public SendMessageCommandHandler(IConversationRepository conversationRepository, IMessageRepository messageRepository, IPresenceService presenceService, IBlockedUserRepository blockedUserRepository)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _presenceService = presenceService;
            _blockedUserRepository = blockedUserRepository;
        }

        public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {

            if (await _blockedUserRepository.IsUserBlockedAsync(request.UserId, request.RecipientId) ||
               await _blockedUserRepository.IsUserBlockedAsync(request.RecipientId, request.UserId))
                throw new UserBlockedException();

            var conversation = await _conversationRepository.GetByUsersAsync((request.UserId), (request.RecipientId));
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = Guid.Parse(request.UserId),
                    User2Id = Guid.Parse(request.RecipientId),
                    LastActivity = DateTime.UtcNow
                };
                await _conversationRepository.CreateAsync(conversation);
            }

            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = Guid.Parse(request.UserId),
                Content = request.Content,
                Status = MessageStatus.Sent
            };

            var createdMessage = await _messageRepository.CreateAsync(message);
            conversation.LastActivity = DateTime.UtcNow;
            await _conversationRepository.UpdateAsync(conversation); // Update LastActivity

            if (await _presenceService.IsUserOnlineAsync(request.RecipientId))
            {
                await _messageRepository.UpdateStatusAsync(createdMessage.Id.ToString(), MessageStatus.Delivered);
                await _presenceService.PublishMessageAsync(request.RecipientId, "ReceiveMessage", new MessageDto
                {
                    Id = createdMessage.Id.ToString(),
                    SenderId = createdMessage.SenderId.ToString(),
                    Content = createdMessage.Content,
                    Status = createdMessage.Status,
                });
            }

            return new MessageDto
            {
                Id = createdMessage.Id.ToString(),
                SenderId = createdMessage.SenderId.ToString(),
                Content = createdMessage.Content,
                Status = createdMessage.Status,
            };
        }
    }
}
