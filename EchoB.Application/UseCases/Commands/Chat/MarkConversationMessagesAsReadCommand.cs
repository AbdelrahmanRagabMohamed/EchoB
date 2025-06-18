using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Chat
{
    public record MarkConversationMessagesAsReadCommand(string conversationId,string userId) : IRequest;

    public class MarkConversationMessagesAsReadCommandHandler : IRequestHandler<MarkConversationMessagesAsReadCommand>
    {
        private readonly IConversationRepository _conversationRepository;
        
        public MarkConversationMessagesAsReadCommandHandler(IConversationRepository conversationRepository )
        {
            _conversationRepository = conversationRepository;
            
        }
        public async Task Handle(MarkConversationMessagesAsReadCommand command, CancellationToken cancellationToken)
        {
            await _conversationRepository.MarkConversationMessagesAsReadAsync(command.conversationId, command.userId,cancellationToken);
        }
    }


}
