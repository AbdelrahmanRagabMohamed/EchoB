using EchoB.Application.DTOs;
using EchoB.Application.DTOs.User;
using EchoB.Application.UseCases.Queries.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EchoB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Get my All Chats 
        /// </summary>
        /// <param name="">User ID</param>
        /// <returns>List of chats</returns>
        [HttpGet("chats")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetChatList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new UnauthorizedAccessException();
            GetChatListQuery query = new GetChatListQuery();
            var chats = await _mediator.Send( query);
            return Ok(chats);
        }


        /// <summary>
        /// Get messages on chat
        /// </summary>
        /// <param name="">chat ID</param>
        /// <returns>List of chats</returns>
        [HttpGet("chat")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetChat()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                throw new UnauthorizedAccessException();
            GetChatMessagesQuery query = new GetChatMessagesQuery();
            var chats = await _mediator.Send(query);
            return Ok(chats);
        }

    }
}
