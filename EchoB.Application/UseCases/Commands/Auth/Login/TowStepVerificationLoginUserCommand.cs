using AutoMapper;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Auth.Login
{
    public class TowStepVerificationLoginUserCommand : IRequest<AuthenticationResultDto>
    {
        public string UserName { get; set; }
        public string Otp { get; set; }
    }
    public class TowStepVerificationLoginUserCommandHandler : IRequestHandler<TowStepVerificationLoginUserCommand, AuthenticationResultDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IOtpService _otpService;

        public TowStepVerificationLoginUserCommandHandler(
            ITokenService tokenService,
            IMapper mapper, UserManager<EchoBUser> userManager, IOtpService otpService)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _otpService = otpService;

        }

        public async Task<AuthenticationResultDto> Handle(TowStepVerificationLoginUserCommand request, CancellationToken cancellationToken)
        {
            // Try to find user by email or username
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null || user.IsDeleted||!user.TwoFactorEnabled)
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




            try
            {
                var contact = request.UserName.Trim();
                if (string.IsNullOrEmpty(contact))
                    return new AuthenticationResultDto { Succeeded = false, Message = "UserName is empty", Token = null };
                var Key = $"2FA-login:{contact}";
                var isValidOtp = await _otpService.VerifyOtp(Key, request.Otp);
                if (!isValidOtp)
                {
                    return new AuthenticationResultDto { Succeeded = false, Message = "Invalid or expired OTP.", Token = null };
                }
                TokenValues token = await _tokenService.GenerateAccessTokenAsync(user);
                // Reset failed access count on successful login
                user.ResetAccessFailedCount();
                user.SetOnlineStatus(true);
                await _userManager.UpdateAsync(user);
                return new AuthenticationResultDto { Succeeded = true,Message = "Login successfully", Token = token };
            }
            catch (Exception ex)
            {
                return new AuthenticationResultDto {Succeeded=false, Message = $"Un Expected Error{ex.Message}", Token = null };
                
            }




        }


    }
}
