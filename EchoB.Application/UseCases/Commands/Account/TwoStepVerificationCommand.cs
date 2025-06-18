using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Account
{
    namespace EchoB.Application.UseCases.Commands
    {
        public class TwoStepVerificationCommand : IRequest<ResultDto>
        {
            public string EmailOrPhone { get; set; } = string.Empty;
        }
        public class TwoStepVerificationCommandHandler : IRequestHandler<TwoStepVerificationCommand, ResultDto>
        {
            
            private readonly ILogger<TwoStepVerificationCommandHandler> _logger;
            private readonly IOtpService _otpService;
            public TwoStepVerificationCommandHandler(
                
                ILogger<TwoStepVerificationCommandHandler> logger,IOtpService otpService)
            {
               
                _logger = logger;
                _otpService = otpService;
            }

            public async Task<ResultDto> Handle(TwoStepVerificationCommand request, CancellationToken cancellationToken)
            {

                var contact = request.EmailOrPhone.Trim();
                if (string.IsNullOrEmpty(contact))
                    return new ResultDto {Succeeded =false,Message = "Contact is empty" };
                var Key = $"tow-step-verify:{contact}";
                try
                {
                    var response = await _otpService.SendOtp(request.EmailOrPhone,Key, cancellationToken);
                    if (string.IsNullOrEmpty(response))
                        return new ResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later." };

                    if (response != "ok")
                    {
                        Console.WriteLine(response);
                        return new ResultDto { Succeeded = false, Message = "Failed to send OTP. Please try again later."};

                    }
                    return new ResultDto { Succeeded = true, Message = "OTP sent successfully." };

                }
                catch
                (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return new ResultDto { Succeeded = false, Message = ex.Message };

                }

            }

            
        }
    }
}

