using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Auth.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ResultDto>
    {
        public string UserName { get; set; } = string.Empty;
    }
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ResultDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly IOtpService _otpService;

        public ForgotPasswordCommandHandler(UserManager<EchoBUser> userManager,
            IOtpService otpService)
        {
            _userManager = userManager;
            _otpService = otpService;
        }

        public async Task<ResultDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
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

            try
            {
                var contact = request.UserName.Trim();
                if (string.IsNullOrEmpty(contact))
                    return new ResultDto { Succeeded = false, Message = "UserName is empty" };
                var Key = $"forgot-password:{contact}";
                var response = await _otpService.SendOtp(request.UserName,Key, cancellationToken);
                if (string.IsNullOrEmpty(response))
                    return new ResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later." };

                if (response != "ok")
                {
                    Console.WriteLine(response);
                    return new ResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later."};

                }

            }
            catch
            {
                Console.WriteLine("Error While Sending otp");

                return new ResultDto { Succeeded = false, Message = "Otp sent failed" };

            }

            return new ResultDto { Succeeded =true, Message = "Otp sent successfully" };
        }

       
    }

}
