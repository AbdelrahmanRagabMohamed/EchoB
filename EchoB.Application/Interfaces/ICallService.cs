using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.Interfaces
{
    public interface ICallService
    {
        Task<CallResponseDto> StartCallAsync(string callerId, CallRequestDto request);
        Task AnswerCallAsync(AnswerRequestDto request);
        Task EndCallAsync(Guid callId);
        Task<CallResponseDto> GetCallStatusAsync(Guid callId);
    }
}
