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
    public class AnswerCallCommand : IRequest
    {
        public string CallId { get; set; }
        public bool Accept { get; set; }
    }
    public class AnswerCallCommandHandler : IRequestHandler<AnswerCallCommand>
    {
        private readonly ICallRepository _callRepository;
        private readonly ISignalRService _signalRService;
        public AnswerCallCommandHandler(ICallRepository callRepository, ISignalRService signalRService)
        {
            _callRepository = callRepository;
            _signalRService = signalRService;
        }
        public async Task Handle(AnswerCallCommand request, CancellationToken cancellationToken)
        {
            var call = await _callRepository.GetByIdAsync(request.CallId) ?? throw new Exception("Call not found");
            call.Status = request.Accept ? CallStatus.Active : CallStatus.Ended;
            if (!request.Accept) call.EndTime = DateTime.UtcNow;
            await _callRepository.UpdateAsync(call);
            await _signalRService.NotifyCallAnswered(call.CallerId.ToString(), call.ReceiverId.ToString(), request.Accept);
        }
    }
}
