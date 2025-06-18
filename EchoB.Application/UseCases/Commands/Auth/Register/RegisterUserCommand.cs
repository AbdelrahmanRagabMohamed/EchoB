using AutoMapper;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using EchoB.Domain.ValidationAttributes;
using EchoB.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Auth.Register
{
    public class RegisterUserCommand : IRequest<ResultDto>
    {
        public string FullName { get; set; } = string.Empty;
        [ContactValidation]
        public string UserName { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ResultDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IOtpService _otpService;

        public RegisterUserCommandHandler(UserManager<EchoBUser> userManager,
            ITokenService tokenService,
            IMapper mapper,
            IOtpService otpService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _otpService = otpService;

        }

        public async Task<ResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null &&  !existingUser.IsDeleted)
                throw new UserAlreadyExistsException(existingUser.UserName);

            var user = new EchoBUser(request.FullName,request.UserName );
            

            try
            {


                try
                {
                    var contact = request.UserName.Trim();
                    if (string.IsNullOrEmpty(contact))
                        return new ResultDto { Succeeded = false, Message = "UserName is empty" };
                    var Key = $"verify-register:{contact}";
                    var response = await _otpService.SendOtp(request.UserName, Key, cancellationToken);
                    if (string.IsNullOrEmpty(response))
                        return new ResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later." };

                    if (response != "ok")
                    {
                        Console.WriteLine(response);
                        return new ResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later." };

                    }

                }
                catch
                {
                    Console.WriteLine("Error While Sending otp");

                    return new ResultDto { Succeeded = false, Message = "Otp sent failed" };

                }
                await _userManager.CreateAsync(user, request.Password);
                return new ResultDto { Succeeded = true, Message = "Otp sent successfully" };
                

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[SaveUserToDatabase] Operation was cancelled.");
                return new ResultDto { Message = "User registered Failed" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveUserToDatabase Error] {ex.Message}");
                return new ResultDto { Message = "An error occurred while saving the user." };
            }
            

            
        }
    }
}

