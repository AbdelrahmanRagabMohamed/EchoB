using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Chat;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Chat
{
    public record EditMessageCommand(string MessageId, string UserId, string NewContent) : IRequest<MessageDto>;
    public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, MessageDto>
    {
        private readonly IMessageRepository _messageRepository;

        public EditMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<MessageDto> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.EditMessageAsync(request.MessageId, request.UserId, request.NewContent);


            

            return new MessageDto
            {
                Id = message.Id.ToString(),
                Content = message.Content,
                SenderId = message.SenderId.ToString(),
                Status = message.Status,
            };
        }
    }

}
