using EchoB.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Chat
{
    public record DeleteMessageCommand(string MessageId, string UserId, bool ForEveryone) : IRequest;
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
    {
        private readonly IMessageRepository _messageRepository;

        public DeleteMessageCommandHandler(IMessageRepository repository)
        {
            _messageRepository = repository;
        }

        public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
             await _messageRepository.DeleteMessageAsync(request.MessageId, request.UserId, request.ForEveryone);

            
        }
    }


}
