using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Call;
using EchoB.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Queries.Call
{
    public class GetCallStatusQuery : IRequest<CallResponseDto>
    {
        public string CallId { get; set; }
    }
    public class GetCallStatusQueryHandler : IRequestHandler<GetCallStatusQuery, CallResponseDto>
    {
        private readonly ICallRepository _callRepository;
        public GetCallStatusQueryHandler(ICallRepository callRepository)
        {
            _callRepository = callRepository;
        }
        public async Task<CallResponseDto> Handle(GetCallStatusQuery request, CancellationToken cancellationToken)
        {
            var call = await _callRepository.GetByIdAsync(request.CallId) ?? throw new Exception("Call not found");
            return new CallResponseDto { CallId = call.Id, Status = call.Status, StartTime = call.StartTime };
        }
    }
}
