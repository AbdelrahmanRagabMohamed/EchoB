using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Chat;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EchoB.Application.UseCases.Queries.Chat
{
    public record GetChatMessagesQuery : IRequest<ChatDto>
    {
        public string ConversationId { get; set; }
        public string OtherUserId { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
    }
    public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, ChatDto>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<EchoBUser> _userManager;
        IBlockedUserRepository _blockedUserRepository;

        public GetChatMessagesQueryHandler(
            IMessageRepository messageRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<EchoBUser> userManager,
            IBlockedUserRepository blockedUserRepository)
            
        {
            _messageRepository = messageRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _blockedUserRepository = blockedUserRepository;
        }

        public async Task<ChatDto> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
                throw new UnauthorizedAccessException();

            var otherUser = await  _userManager.FindByIdAsync(request.OtherUserId);
            if (otherUser == null)
                throw new UserNotFoundException(request.OtherUserId);

            var messages = await _messageRepository.GetByConversationAsync(request.ConversationId, userId, request.Skip, request.Take);
            var messagesDto = messages.Select(m => new MessageDto
            {
                Id = m.Id.ToString(),
                SenderId = m.SenderId.ToString(),
                Content = m.Content,
                Status = m.Status,
                MessageType = m.MessageType,
                ReplyToMessageId = m.ReplyToMessageId?.ToString(),
                Reactions = m.Reactions.Select(r => new MessageReactionDto
                {
                    MessageId = r.MessageId,
                    UserId = r.UserId,
                    ReactionType = r.ReactionType,
                    CreatedAt = r.CreatedAt
                }).ToList()
            }).ToList();
            return new ChatDto
            {
                ConversationId = request.ConversationId,
                OtherId = otherUser.Id.ToString(),
                OtherFullName = otherUser.FullName,
                OtherUserImage = otherUser.ProfilePictureUrl,
                OtherUserName = otherUser.UserName,
                IsBlocked = await _blockedUserRepository.IsUserBlockedAsync(userId, otherUser.Id.ToString()),
                IsOnline = otherUser.IsOnline,
                Messages = messagesDto
            };
        }
    }
}
