using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Auth.ForgotPassword
{
    public class ResetPasswordCommand : IRequest<AuthenticationResultDto>
    {
        public string UserName { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, AuthenticationResultDto>
    {

        private readonly IOtpService _otpService;
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ITokenService _tokenService;

        public ResetPasswordCommandHandler(
            UserManager<EchoBUser> userManager,
            IOtpService otpService,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _otpService = otpService;
            _tokenService = tokenService;
        }

        public async Task<AuthenticationResultDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {

            // Try to find user by email or username
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null || user.IsDeleted)
                throw new UserNotFoundException(request.UserName);

            // Check if account is locked
            if (user.IsLockedOut)
            {
                if (user.LockoutEnd != null)
                    throw new UserAccountLockedException(user.LockoutEnd.Value);
            }


            if (!user.IsVerified)
            {
                throw new UserNotVerifiedException();
            }
            if (user.IsLocked)
            {

                throw new UserAccountLockedException();
            }

            var contact = request.UserName.Trim();
            if (string.IsNullOrEmpty(contact))
                return new AuthenticationResultDto { Succeeded = false, Message = "UserName is empty",Token = null };
            var Key = $"forgot-password:{contact}";
            var isValidOtp = await _otpService.VerifyOtp(Key, request.Otp);
            if (!isValidOtp)
            {
                return new AuthenticationResultDto {Succeeded = false, Message = "Invalid or expired OTP.", Token = null };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
                return new AuthenticationResultDto {Succeeded =false, Message = $"Failed to reset password: {errorMessage}", Token = null };
            }
            user.TokenVersion++;
            TokenValues accessToken = await _tokenService.GenerateAccessTokenAsync(user);

            // Reset failed access count on successful login
            await _userManager.ResetAccessFailedCountAsync(user);
            user.SetOnlineStatus(true);
            await _userManager.UpdateAsync(user);


            return new AuthenticationResultDto
            {
                Message = "Account verified successfully.",
                Token = accessToken
            };
        }
    }


}
