using AutoMapper;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using EchoB.Domain.ValidationAttributes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Auth.Login
{
    public class LoginUserCommand : IRequest<AuthenticationResultDto>
    {
        [ContactValidation]
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationResultDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IOtpService _otpService;

        public LoginUserCommandHandler(
            ITokenService tokenService,
            IMapper mapper,UserManager<EchoBUser> userManager,IOtpService otpService)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _otpService = otpService;

        }

        public async Task<AuthenticationResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Try to find user by email or username
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null || user.IsDeleted)
                throw new UserNotFoundException(request.UserName);

            // Check if account is locked
            if (user.IsLockedOut)
            {
                if(user.LockoutEnd != null)
                    throw new UserAccountLockedException(user.LockoutEnd.Value);
            }
           

            if (!user.IsVerified)
            {
                throw new UserNotVerifiedException();
            }
            if (user.IsLocked) {

                throw new UserAccountLockedException();
            }

            bool found = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!found)
                throw new InvalidUserOperationException("Invalid email/username or password.");


            if (user.TwoFactorEnabled)

            {
                try
                {
                    var contact = request.UserName.Trim();
                    if (string.IsNullOrEmpty(contact))
                        return new AuthenticationResultDto { Succeeded = false, Message = "UserName is empty",Token =null };
                    var Key = $"2FA-login:{contact}";
                    var response = await _otpService.SendOtp(request.UserName,Key, cancellationToken);
                    if (string.IsNullOrEmpty(response))
                        return new AuthenticationResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later.", Token = null };

                    if (response != "ok")
                    {
                        Console.WriteLine(response);
                        return new AuthenticationResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later.", Token = null };
   
                    }

                }
                catch
                {
                    Console.WriteLine("Error While Sending otp");

                    return new AuthenticationResultDto { Succeeded = false, Message = "Otp sent failed", Token = null };

                }

                return new AuthenticationResultDto {Succeeded =true, Message = "OTP sent successfully", Token = null };
            }
            TokenValues token = await _tokenService.GenerateAccessTokenAsync(user);
            // Reset failed access count on successful login
            user.ResetAccessFailedCount();
            user.SetOnlineStatus(true);
            await _userManager.UpdateAsync(user);
            return new AuthenticationResultDto { Message = "Login successfully",Token = token };
            



        }


    }
}

