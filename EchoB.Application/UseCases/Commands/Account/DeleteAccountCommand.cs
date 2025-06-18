using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.UseCases.Queries.Profile;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Account
{
    public class DeleteAccountCommand : IRequest<ResultDto>
    {
        
        public required string Password { get; set; }
    }
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, ResultDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        ILogger<GetMyProfileQueryHandler> _logger;


        public DeleteAccountCommandHandler(UserManager<EchoBUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetMyProfileQueryHandler> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ResultDto> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
                throw new UnauthorizedAccessException();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found for ID: {UserId}", userId);
                throw new UserNotFoundException(userId);
            }
            // Check if account is locked
            if (user.IsLockedOut)
            {
                if (user.LockoutEnd != null)
                    throw new UserAccountLockedException(user.LockoutEnd.Value);
            }
            try
            {
                var result = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!result)
                    return new ResultDto { Succeeded = false, Message = "Password wrong!" };
                user.IsDeleted = true;
                var response = await _userManager.UpdateAsync(user);
                if(!response.Succeeded)
                    return new ResultDto { Succeeded = false, Message = "Error when save to database!" };
                return new ResultDto { Succeeded = true, Message = "Deleted successfully." };

            }
            catch(Exception ex) 
            {
                return new ResultDto { Succeeded = false, Message = $"Error :{ex.Message}" };

            }

        }
    }

}
