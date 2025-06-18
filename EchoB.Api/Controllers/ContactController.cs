using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.UseCases.Queries.Contact;
using EchoB.Application.UseCases.Queries.Profile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EchoB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IMediator mediator, ILogger<ContactController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get Matches Contact
        /// </summary>
        /// <param name="query">List of contact</param>
        /// <returns>List of user data</returns>
        [HttpGet("match-contacts")]
        [ProducesResponseType(typeof(List<RegisteredContactDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<RegisteredContactDTO>>> GetUserProfile(MatchContactsQuery query)
        {

            var result = await _mediator.Send(query);

            return Ok(result);
        }

    }
}
