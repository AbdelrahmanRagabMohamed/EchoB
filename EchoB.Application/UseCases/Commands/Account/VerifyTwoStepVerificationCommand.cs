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
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Account
{
    public class VerifyTwoStepVerificationCommand : IRequest<ResultDto>
    {
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
    public class VerifyTwoStepVerificationCommandHandler : IRequestHandler<VerifyTwoStepVerificationCommand, ResultDto>
    {
        private readonly IOtpService _otpService;
        private readonly ILogger<VerifyTwoStepVerificationCommandHandler> _logger;

        public VerifyTwoStepVerificationCommandHandler(IOtpService otpService, ILogger<VerifyTwoStepVerificationCommandHandler> logger)
        {
            _logger = logger;
            _otpService = otpService;
        }

        public async Task<ResultDto> Handle(VerifyTwoStepVerificationCommand request, CancellationToken cancellationToken)
        {
            var contact = request.EmailOrPhone.Trim();
            if (string.IsNullOrEmpty(contact))
                return new ResultDto { Succeeded = false, Message = "Contact is empty" };
            var Key = $"tow-step-verify:{contact}";

            var isValidOtp = await _otpService.VerifyOtp(Key, request.Code);
            if (!isValidOtp)
            {
                _logger.LogWarning("Verification code expired or not found for {Contact}", contact);

                return new ResultDto { Succeeded = false, Message = "Invalid or expired OTP." };
            }

           
            return new ResultDto { Succeeded = true, Message = "OTP sent successfully."};

        }
    }
}
