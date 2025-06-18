using EchoB.Application.DTOs;
using EchoB.Application.UseCases.Commands.Chat;
using EchoB.Domain.Entities;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.User
{
    public record UpdateUserStatusCommand(string UserId,bool IsOnline) : IRequest;

    public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand>
    {
        private readonly UserManager<EchoBUser> _userManager;
      
        public UpdateUserStatusCommandHandler(UserManager<EchoBUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user =await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new UnauthorizedAccessException();
            user.IsOnline = request.IsOnline;
            await _userManager.UpdateAsync(user);
        }
    }
}