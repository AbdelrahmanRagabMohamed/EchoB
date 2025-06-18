using EchoB.Api.Filters;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Auth;
using EchoB.Application.UseCases.Commands.Auth.ForgotPassword;
using EchoB.Application.UseCases.Commands.Auth.Login;
using EchoB.Application.UseCases.Commands.Auth.Register;
using EchoB.Application.UseCases.Commands.Auth.UnlockAccount;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EchoB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="command">User registration data</param>
        /// <returns>Authentication result with tokens, message and status </returns>

        [HttpPost("register")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResultDto>> Register([FromBody] RegisterUserCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);

            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }
        /// <summary>
        /// Verify account
        /// </summary>
        /// <param name="command">User verification data </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("verify-account")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthenticationResultDto>> VerifyAccount([FromBody] VerifyUserAccountCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }

        /// <summary>
        /// resend otp
        /// </summary>
        /// <param name="command"> username that send to otp </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("resend-otp")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResultDto>> ResendOtp([FromBody] ResendOtpForRegisterCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }

        /// <summary>
        /// Authenticate user and get tokens
        /// </summary>
        /// <param name="command">Login credentials</param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("login")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthenticationResultDto>> Login([FromBody] LoginUserCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }

        /// <summary>
        /// 2FA 
        /// </summary>
        /// <param name="command"> Email or phone and otp  </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("tow-step-verification-login")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthenticationResultDto>> TowStepVerification([FromBody] TowStepVerificationLoginUserCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }
        /// <summary>
        /// Forgot passowrd
        /// </summary>
        /// <param name="command"> username that send to otp </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("forgot-password")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResultDto>> ForgotPassowrd([FromBody] ForgotPasswordCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }


        /// <summary>
        /// Reset passowrd
        /// </summary>
        /// <param name="command"> username, otp and new passowrd </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("reset-passowrd")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthenticationResultDto>> ResetPassowrd([FromBody] ResetPasswordCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }

        /// <summary>
        /// UnLock user account
        /// </summary>
        /// <param name="command"> username that send to otp </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("unlock-account")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResultDto>> UnlockAccount([FromBody] UnlockUserAccountCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }

        /// <summary>
        /// Reset user account 
        /// </summary>
        /// <param name="command"> username, otp and new passowrd </param>
        /// <returns>Authentication result with tokens, message and status </returns>
        [HttpPost("reset-account")]
        [ServiceFilter(typeof(IpBlockingFilter))]
        [ProducesResponseType(typeof(AuthenticationResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthenticationResultDto>> ResetAccount([FromBody] ResetUserAccountCommand command)
        {
            _logger.LogInformation("User registration attempt for username: {UserName}", command.UserName);

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Message);
            _logger.LogInformation("Sent otp to username: {UserName}", command.UserName);

            return Ok(result);
        }

       
    }
}
