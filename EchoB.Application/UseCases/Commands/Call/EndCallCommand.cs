using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Call
{
    public class EndCallCommand : IRequest
    {
        public string CallId { get; set; }
    }
    public class EndCallCommandHandler : IRequestHandler<EndCallCommand>
    {
        private readonly ICallRepository _callRepository;
        private readonly ISignalRService _signalRService;
        public EndCallCommandHandler(ICallRepository callRepository, ISignalRService signalRService)
        {
            _callRepository = callRepository;
            _signalRService = signalRService;
        }
        public async Task Handle(EndCallCommand request, CancellationToken cancellationToken)
        {
            var call = await _callRepository.GetByIdAsync(request.CallId) ?? throw new Exception("Call not found");
            call.Status = CallStatus.Ended;
            call.EndTime = DateTime.UtcNow;
            await _callRepository.UpdateAsync(call);
            await _signalRService.NotifyCallEnded(call.CallerId.ToString(), call.ReceiverId.ToString());
        }
    }
}
