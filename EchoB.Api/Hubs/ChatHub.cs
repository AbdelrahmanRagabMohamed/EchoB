using EchoB.Application.UseCases.Commands.Block;
using EchoB.Application.UseCases.Commands.Chat;
using EchoB.Application.UseCases.Commands.NotificationCommand;
using EchoB.Application.UseCases.Commands.User;
using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using Polly;
using Twilio.TwiML.Messaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EchoB.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IPresenceService _presenceService;
        private readonly INotificationRepository _notificationRepository;
        public ChatHub(IMediator mediator, IPresenceService presenceService, INotificationRepository notificationRepository)
        {
            _mediator = mediator;
            _presenceService = presenceService;
            _notificationRepository = notificationRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _presenceService.AddConnectionAsync(userId, Context.ConnectionId);
            await _mediator.Send(new UpdateUserStatusCommand(userId, true));
            await SendPendingNotificationsAsync(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _presenceService.RemoveConnectionAsync(userId, Context.ConnectionId);
            await _mediator.Send(new UpdateUserStatusCommand(userId, false));
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string recipientId, string content)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            var message = await _mediator.Send(new SendMessageCommand(userId, recipientId, content));
            await Clients.User(recipientId).SendAsync("ReceiveMessage", message);
            await _presenceService.PublishMessageAsync(userId, "MessageStatusUpdated", new { MessageId = message.Id, Status = message.Status });
            await Clients.User(recipientId).SendAsync("ChatListUpdated");
            var notification = await _mediator.Send(new CreateNotificationCommand(recipientId, NotificationType.Message, $"New message", message.Content, null));
            var isOnline = await _presenceService.IsUserOnlineAsync(recipientId);
            if (isOnline)
            {
                await DeliverNotificationAsync(notification);
            }

        }

        public async Task UpdateMessageStatus(string messageId, string status)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _mediator.Send(new UpdateMessageStatusCommand(messageId, Enum.Parse<MessageStatus>(status)));
            await _presenceService.PublishMessageAsync(userId, "MessageStatusUpdated", new { MessageId = messageId, Status = status });

        }
        public async Task MarkMessagesAsRead(string conversationId, string recipientId)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _mediator.Send(new MarkConversationMessagesAsReadCommand(conversationId, userId));
            await _presenceService.PublishMessageAsync(recipientId, "MessagesMarkedAsRead", new { ConversationId = conversationId });
            await Clients.User(recipientId).SendAsync("ChatListUpdated");
        }
        public async Task ReactToMessage(string messageId, string recipientId, ReactionType reactionType)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            var reaction = await _mediator.Send(new AddMessageReactionCommand(messageId, userId, reactionType));
            await Clients.User(recipientId).SendAsync("MessageReactionUpdated", reaction);
            await _presenceService.PublishMessageAsync(userId, "MessageReactionUpdated", reaction);
            await Clients.User(recipientId).SendAsync("ChatListUpdated");
            var notification = await _mediator.Send(new CreateNotificationCommand(recipientId, NotificationType.Reaction, $"Reacte on message", reactionType.ToString(), null));
            var isOnline = await _presenceService.IsUserOnlineAsync(recipientId);
            if (isOnline)
            {
                await DeliverNotificationAsync(notification);
            }
        }

        public async Task DeleteMessage(string messageId, string recipientId, bool forEveryone = false)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _mediator.Send(new DeleteMessageCommand(messageId, userId, forEveryone));
            await Clients.User(recipientId).SendAsync("MessageDeleted", messageId);
            await _presenceService.PublishMessageAsync(userId, "MessageDeleted", messageId);
        }
        public async Task EditMessage(string messageId, string recipientId, string newContent)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            var message = await _mediator.Send(new EditMessageCommand(messageId, userId, newContent));
            await Clients.User(recipientId).SendAsync("MessageUpdated", message);
            await _presenceService.PublishMessageAsync(userId, "MessageUpdated", message);
        }
        public async Task BlockUser(string userIdToBlock)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _mediator.Send(new BlockUserCommand(userId, userIdToBlock));
            await Clients.User(userIdToBlock).SendAsync("UserBlocked", new { BlockedByUserId = userId });
            await _presenceService.PublishMessageAsync(userId, "UserBlocked", new { BlockedByUserId = userId });

        }
        public async Task UnBlockUser(string userIdToUnBlock)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException();
            await _mediator.Send(new UnBlockUserCommand(userId, userIdToUnBlock));
            await Clients.User(userIdToUnBlock).SendAsync("UserUnBlocked", new { UnBlockedByUserId = userId });
            await _presenceService.PublishMessageAsync(userId, "UserUnBlocked", new { UnBlockedByUserId = userId });

        }
        private string? GetUserId()
        {
            return Context.UserIdentifier;
        }
        private async Task DeliverNotificationAsync(Notification notification)
        {
            var userConnections = await _presenceService
                .GetConnectionsAsync(notification.UserId.ToString());

            if (userConnections.Any())
            {
                await Clients.Users(userConnections)
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


    
     private async Task SendPendingNotificationsAsync(string userId)
        {
            var pendingNotifications = await _notificationRepository
                .GetPendingNotificationsAsync(userId);

            foreach (var notification in pendingNotifications)
            {
                await Clients.Caller.SendAsync("NotificationReceived", notification);
                await MarkAsDeliveredAsync(notification.Id.ToString());
            }
        }
    }
}

