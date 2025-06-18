using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.Interfaces
{
    public interface IOtpService
    {
        Task<string> SendOtp(string Contact, CancellationToken cancellationToken = default);
        Task<string> SendOtp(string Contact,string key, CancellationToken cancellationToken = default);

        Task<bool> VerifyOtp(string Contact, string Code, CancellationToken cancellationToken = default);
    }
}
