using EchoB.Domain.Interfaces;
using EchoB.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using EchoB.Domain.Entities;

namespace EchoB.Application.UseCases.Commands.Block
{
    public record BlockUserCommand(string UserId,string UserIdToBlock) : IRequest<bool>;
    

    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, bool>
    {
        private readonly IBlockedUserRepository _blockedUserRepository;

        public BlockUserCommandHandler(IBlockedUserRepository blockedUserRepository)
        {
            _blockedUserRepository = blockedUserRepository;
        }

        public async Task<bool> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            

            await _blockedUserRepository.BlockUserAsync(request.UserId,request.UserIdToBlock);
            

            return true;
        }
    }

   
}

