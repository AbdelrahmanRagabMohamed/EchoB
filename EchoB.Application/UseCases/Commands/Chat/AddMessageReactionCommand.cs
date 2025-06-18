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
    public record AddMessageReactionCommand(string MessageId, string UserId, ReactionType ReactionType) : IRequest<MessageReactionDto>;
    public class AddMessageReactionCommandHandler : IRequestHandler<AddMessageReactionCommand, MessageReactionDto>
    {
        private readonly IMessageRepository _messageRepository;

        public AddMessageReactionCommandHandler(IMessageRepository repository)
        {
            _messageRepository = repository;
        }

        public async Task<MessageReactionDto> Handle(AddMessageReactionCommand request, CancellationToken cancellationToken)
        {
            var messageReaction = await _messageRepository.AddMessageReactionAsync(request.MessageId, request.UserId, request.ReactionType);

            

            
            return new MessageReactionDto
            {
                MessageId = messageReaction.MessageId,
                UserId = messageReaction.UserId,
                ReactionType = messageReaction.ReactionType,
              
            };
        }
    }
}
