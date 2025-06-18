using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.DTOs.User;
using EchoB.Application.UseCases.Commands.Account;
using EchoB.Application.UseCases.Commands.Auth.Login;
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
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="query">User ID</param>
        /// <returns>User data</returns>
        [HttpGet("user-profile{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetUserProfile(string id)
        {
            GetUserProfileQuery query= new GetUserProfileQuery();
            query.UserId = id;
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get my profile
        /// </summary>
        /// <param name="query">Get my profile</param>
        /// <returns>User data</returns>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            GetMyProfileQuery query= new GetMyProfileQuery();

            var result = await _mediator.Send(query);

            return Ok(result);
        }
        /// <summary>
        /// Update my profile
        /// </summary>
        /// <param name="command">Update my profile</param>
        /// <returns>User data</returns>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateUserProfileCommand command)
        {

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Change account password
        /// </summary>
        /// <param name="command"> Change password (currentPassword,newPassword,signOutFromAllDevices)</param>
        /// <returns>AuthenticationResult{status,message,token}</returns>
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthenticationResultDto>> ChangePassword(ChangePasswordCommand command)
        {

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// 2FA send otp to contact
        /// </summary>
        /// <param name="command"> Send otp to email or phone (email/phoneNumber)</param>
        /// <returns>Result{status,message}</returns>
        [HttpPost("tow-step-virification")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResultDto>> TowStepVerification(TowStepVerificationLoginUserCommand command)
        {

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        /// Verify 2FA contact
        /// </summary>
        /// <param name="command"> Verify email or phone (email/phoneNumber, otp)</param>
        /// <returns>Result{status,message}</returns>
        [HttpPost("verify-tow-step-virification")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResultDto>> VerifyTowStepVerification(VerifyTwoStepVerificationCommand command)
        {

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
