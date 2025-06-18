using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using EchoB.Domain.ValidationAttributes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Auth.Register
{
    public class VerifyUserAccountCommand : IRequest<AuthenticationResultDto>
    {
        [ContactValidation]
        public string UserName { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;

        public VerifyUserAccountCommand(string username, string otp)
        {
            UserName = username;
            Otp = otp;
        }
    }
    public class VerifyUserAccountCommandHandler : IRequestHandler<VerifyUserAccountCommand, AuthenticationResultDto>
    {
        private readonly IOtpService _otpService;
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ITokenService _tokenService;

        public VerifyUserAccountCommandHandler(UserManager<EchoBUser> userManager, IOtpService otpService, ITokenService tokenService)
        {
            _userManager = userManager;
            _otpService = otpService;
            _tokenService = tokenService;
        }

        public async Task<AuthenticationResultDto> Handle(VerifyUserAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || user.IsDeleted)
                throw new UserNotFoundException(request.UserName);
            // Check if account is locked
            if (user.IsLockedOut)
            {
                if (user.LockoutEnd != null)
                    throw new UserAccountLockedException(user.LockoutEnd.Value);
            }
            if (user.IsLocked)
                throw new UserAccountLockedException();
            if (user.IsVerified)
                throw new UserAlreadyVerifiedException(user.UserName);

            var contact = request.UserName.Trim();
            if (string.IsNullOrEmpty(contact))
                return new AuthenticationResultDto { Succeeded = false, Message = "UserName is empty",Token = null };
            var Key = $"verify-register:{contact}";
            var isValidOtp = await _otpService.VerifyOtp(Key, request.Otp);
            if (!isValidOtp)
            {
                return new AuthenticationResultDto
                {
                    Succeeded = false,
                    Message = "Invalid or expired OTP.",
                    Token = null
                };
            }

            user.IsVerified = true;
            if (IsPhone(user.UserName))
                user.PhoneNumberConfirmed = true;
            else
                user.EmailConfirmed = true;
                TokenValues token = await _tokenService.GenerateAccessTokenAsync(user);
            // Reset failed access count on successful login
            await _userManager.ResetAccessFailedCountAsync(user);
            user.SetOnlineStatus(true);
            await _userManager.UpdateAsync(user);

            return new AuthenticationResultDto
            {
                Succeeded =true,
                Message = "Account verified successfully.",
                Token = token
            };
        }
        private bool IsPhone(string primaryContact)
        {
            string phonePattern = @"^\+?[0-9]{10,15}$";
            if (Regex.IsMatch(primaryContact, phonePattern))
                return true;
            return false;
        }
    }


}
