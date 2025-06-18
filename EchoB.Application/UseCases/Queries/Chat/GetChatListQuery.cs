using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Chat;
using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Queries.Chat
{
    public record GetChatListQuery : IRequest<List<ChatListItemDto>>
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }

    public class GetChatListQueryHandler : IRequestHandler<GetChatListQuery, List<ChatListItemDto>>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<EchoBUser> _userManager;
        public GetChatListQueryHandler(IConversationRepository conversationRepository,
            IMessageRepository messageRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<EchoBUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _userManager = userManager;
        }

        public async Task<List<ChatListItemDto>> Handle(GetChatListQuery request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
                throw new UnauthorizedAccessException();
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId, request.Page * request.PageSize, request.PageSize);
            var result = new List<ChatListItemDto>();

            foreach (var conversation in conversations)
            {
                var otherUser = conversation.User1Id.ToString() == userId ? conversation.User2 : conversation.User1;
                var lastMessage = conversation.Messages.FirstOrDefault();
                var unreadCount = await _messageRepository.GetByConversationAsync(conversation.Id.ToString(),userId, 0, int.MaxValue)
                    .ContinueWith(t => t.Result.Count(m => m.Status != MessageStatus.Read && m.SenderId.ToString() != userId));

                result.Add(new ChatListItemDto
                {
                    ConversationId = conversation.Id.ToString(),
                    OtherUserName = otherUser.UserName,
                    OtherUserImage = otherUser.ProfilePictureUrl,
                    LastMessageContent = lastMessage?.Content,
                    LastMessageTimestamp = lastMessage?.CreatedAt ?? conversation.LastActivity,
                    MessageStatus = lastMessage?.Status.ToString(),
                    UnreadCount = unreadCount
                });
            }

            return result;
        }
    }
}
