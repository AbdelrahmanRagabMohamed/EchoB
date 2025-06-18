using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Block
{
    public record UnBlockUserCommand(string UserId,string UserIdToUnblock) : IRequest<bool>;
  

    public class UnBlockUserCommandHandler : IRequestHandler<UnBlockUserCommand, bool>
    {
        private readonly IBlockedUserRepository _blockedUserRepository;

        public UnBlockUserCommandHandler(IBlockedUserRepository blockedUserRepository)
        {
            _blockedUserRepository = blockedUserRepository;
        }

        public async Task<bool> Handle(UnBlockUserCommand request, CancellationToken cancellationToken)
        {
            

            await _blockedUserRepository.UnblockUserAsync(request.UserId,request.UserIdToUnblock);


            return true;
        }
    }
}
