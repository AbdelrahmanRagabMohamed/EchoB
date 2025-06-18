using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.NotificationCommand
{
    public record CreateNotificationCommand(string UserId, NotificationType Type,string Title,string Content,string? Data) : IRequest<Notification>;
    
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Notification>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IPresenceService _presenceService;


        public CreateNotificationCommandHandler(
            INotificationRepository notificationRepository,
            IPresenceService presenceService
            )
        {
            _notificationRepository = notificationRepository;

            _presenceService = presenceService;
        }


        public async Task<Notification> Handle(CreateNotificationCommand command, CancellationToken cancellationToken)
        {
            var notification = Notification.Create(
                Guid.Parse(command.UserId),
                command.Type,
                command.Title,
                command.Content,
                command.Data
            );
            await _notificationRepository.AddAsync(notification);
            return notification;

        }



    }
}
