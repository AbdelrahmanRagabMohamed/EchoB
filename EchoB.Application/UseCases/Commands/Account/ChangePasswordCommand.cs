using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Account
{
    public class ChangePasswordCommand : IRequest<AuthenticationResultDto>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public bool SignOutFromAllDevices { get; set; }=false;
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, AuthenticationResultDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        public ChangePasswordCommandHandler(UserManager<EchoBUser> userManager,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

        }

        public async Task<AuthenticationResultDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
                throw new UnauthorizedAccessException();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UserNotFoundException(userId);

            // Check if account is locked
            if (user.IsLockedOut)
            {
                if (user.LockoutEnd != null)
                    throw new UserAccountLockedException(user.LockoutEnd.Value);
            }
            // Change password (this will increment token version)
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return new AuthenticationResultDto { Succeeded = false,Message = $"Error : {result.Errors}", Token = null };
            if (request.SignOutFromAllDevices)
            {
                user.TokenVersion++; 
                TokenValues token = await _tokenService.GenerateAccessTokenAsync(user);
                // Reset failed access count on successful login
                user.ResetAccessFailedCount();
                await _userManager.UpdateAsync(user);
            }

            return new AuthenticationResultDto { Succeeded = true, Message = $"Change Password successfully", Token = null };

        }
    }
}

