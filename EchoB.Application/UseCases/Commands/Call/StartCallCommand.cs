using EchoB.Application.Common;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Call;
using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Call
{
    public class StartCallCommand : IRequest<CallResponseDto>
    {
        public string CallerId { get; set; }
        public string ReceiverId { get; set; }
    }
    public class StartCallCommandHandler : IRequestHandler<StartCallCommand, CallResponseDto>
    {
        private readonly ICallRepository _callRepository;
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ISignalRService _signalRService;
        private readonly IBlockedUserRepository _blockedUserRepository;
        public StartCallCommandHandler(ICallRepository callRepository, UserManager<EchoBUser> userManager, ISignalRService signalRService, IBlockedUserRepository blockedUserRepository)
        {
            _callRepository = callRepository;
            _signalRService = signalRService;
            _userManager = userManager;
            _blockedUserRepository = blockedUserRepository;
        }
        public async Task<CallResponseDto> Handle(StartCallCommand request, CancellationToken cancellationToken)
        {
            var caller = await _userManager.FindByIdAsync(request.CallerId) ?? throw new UserNotFoundException("Caller not found");
            var receiver = await _userManager.FindByIdAsync(request.ReceiverId) ?? throw new UserNotFoundException("Receiver not found");
            if (await _blockedUserRepository.IsUserBlockedAsync(request.CallerId, request.ReceiverId) ||
                await _blockedUserRepository.IsUserBlockedAsync(request.ReceiverId, request.CallerId))
                throw new UserBlockedException();
            var call = new CallSession
            {
                Id = Guid.NewGuid(),
                CallerId = Guid.Parse(request.CallerId),
                ReceiverId = Guid.Parse(request.ReceiverId),
                Status = CallStatus.Pending,
                StartTime = DateTime.UtcNow
            };
            await _callRepository.AddAsync(call);
            await _signalRService.NotifyCallStarted(request.CallerId, request.ReceiverId, call.Id.ToString());
            return new CallResponseDto { CallId = call.Id, Status = call.Status, StartTime = call.StartTime };
        }
    }
}
