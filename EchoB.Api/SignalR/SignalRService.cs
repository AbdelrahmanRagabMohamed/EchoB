using EchoB.Api.Hubs;
using EchoB.Application.UseCases.Commands.NotificationCommand;
using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Repositories;
using EchoB.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Twilio.TwiML.Messaging;

namespace EchoB.Api.SignalR
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<CallHub> _hubContext;
        private readonly IPresenceService _presenceService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMediator _mediator;
        public SignalRService(IHubContext<CallHub> hubContext, IPresenceService presenceService, INotificationRepository notificationRepository,IMediator mediator)
        {
            _hubContext = hubContext;
            _presenceService = presenceService;
            _notificationRepository = notificationRepository;
            _mediator = mediator;

        }
        public async Task NotifyCallStarted(string callerId, string receiverId, string callId)
        {
            await _hubContext.Clients.User(receiverId).SendAsync("CallStarted", callerId, callId);
            var isOnline = await _presenceService.IsUserOnlineAsync(receiverId);
            if (isOnline)
            {
                var notification = await _mediator.Send(new CreateNotificationCommand(receiverId, NotificationType.CallInvite, $"Call notification", " ", callId));

                await DeliverNotificationAsync(notification);
            }
            await _mediator.Send(new CreateNotificationCommand(receiverId, NotificationType.CallInvite, $"Missed mall notification", " ", callId));

        }
        public async Task NotifyCallAnswered(string callerId, string receiverId, bool accept)
        {
            await _hubContext.Clients.User(callerId).SendAsync("CallAnswered", receiverId, accept);
        }
        public async Task NotifyCallEnded(string callerId, string receiverId)
        {
            await _hubContext.Clients.Users(new[] { callerId, receiverId }).SendAsync("CallEnded");
        }
        public async Task SendOffer(string receiverId, string callerId, object offer)
        {
            await _hubContext.Clients.User(receiverId).SendAsync("ReceiveOffer", callerId, offer);
        }
        public async Task SendAnswer(string callerId, object answer)
        {
            await _hubContext.Clients.User(callerId).SendAsync("ReceiveAnswer", answer);
        }
        public async Task SendIceCandidate(string userId, object candidate)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveIceCandidate", candidate);
        }


        private async Task DeliverNotificationAsync(Notification notification)
        {
            var userConnections = await _presenceService
                .GetConnectionsAsync(notification.UserId.ToString());

            if (userConnections.Any())
            {
                await _hubContext.Clients.Users(userConnections)
                    .SendAsync("NotificationReceived", new
                    {
                        notification.Id,
                        notification.Type,
                        notification.Title,
                        notification.Content,
                        notification.CreatedAt
                    });

                // Mark as delivered
                await MarkAsDeliveredAsync(notification.Id.ToString());

            }
        }

        private async Task MarkAsDeliveredAsync(string notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification != null && notification.Status != NotificationStatus.Delivered)
            {
                notification.Status = NotificationStatus.Delivered;
                await _notificationRepository.UpdateAsync(notification);
            }
        }
    }
}
