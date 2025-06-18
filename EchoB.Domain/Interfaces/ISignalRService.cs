using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface ISignalRService
    {
        Task NotifyCallStarted(string callerId, string receiverId, string callId);
        Task NotifyCallAnswered(string callerId, string receiverId, bool accept);
        Task NotifyCallEnded(string callerId, string receiverId);
        Task SendOffer(string receiverId, string callerId, object offer);
        Task SendAnswer(string callerId, object answer);
        Task SendIceCandidate(string userId, object candidate);
    }
}
