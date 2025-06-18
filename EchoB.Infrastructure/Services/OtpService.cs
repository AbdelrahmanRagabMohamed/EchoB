using DeltaCore.EmailService;
using DeltaCore.SMSService;
using EchoB.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly ISMSService _smsservice;
        private readonly IEmailService _emailService;
        public OtpService(ISMSService smsservice, IEmailService emailService)
        {
            _smsservice = smsservice;
            _emailService = emailService;
        }

        private bool IsPhone(string primaryContact)
        {
            string phonePattern = @"^\+?[0-9]{10,15}$";
            if (Regex.IsMatch(primaryContact, phonePattern))
                return true;
            return false;
        }
        public async Task<string> SendOtp(string Contact, CancellationToken cancellationToken = default)
        {
            bool isPhone = IsPhone(Contact);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (isPhone)
                {
                    await _smsservice.SendOTPAsync(Contact, "sms", cancellationToken);
                }
                else
                {
                    bool emailSent = await _emailService.SendOTPAsync(Contact);
                    if (!emailSent)
                        return "An error occurred. Please try again later.";
                }

                return "ok";
            }
            catch (OperationCanceledException)
            {
                return "Error: Request was cancelled.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        
        }

        public async Task<bool> VerifyOtp(string Contact, string Code, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrWhiteSpace(Contact) || string.IsNullOrWhiteSpace(Code))
                throw new ArgumentException("Primary contact and verification code are required.");

            bool isPhone = IsPhone(Contact);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return isPhone
                    ? await _smsservice.VerifyOTPAsync(Contact, Code, cancellationToken)
                    : await _emailService.VerifyOTP(Contact, Code);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[VerifyCode] Operation was cancelled.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VerifyCode Error] {ex.Message}");
                return false;
            }
        }

        public async Task<string> SendOtp(string Contact, string key, CancellationToken cancellationToken = default)
        {

            bool isPhone = IsPhone(Contact);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (isPhone)
                {
                    await _smsservice.SendOTPAsync(Contact,key, "sms", cancellationToken);
                }
                else
                {
                    bool emailSent = await _emailService.SendOTPAsync(Contact,key);
                    if (!emailSent)
                        return "An error occurred. Please try again later.";
                }

                return "ok";
            }
            catch (OperationCanceledException)
            {
                return "Error: Request was cancelled.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
