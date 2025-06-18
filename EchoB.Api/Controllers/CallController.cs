using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Call;
using EchoB.Application.UseCases.Commands.Call;
using EchoB.Application.UseCases.Queries.Call;
using EchoB.Domain.AI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace EchoB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CallController : ControllerBase
    {
        private readonly IMediator _mediator;
        ISignLanguageProcessor _signLanguageProcessor;
        public CallController(IMediator mediator,ISignLanguageProcessor signLanguageProcessor)
        {
            _mediator = mediator;
            _signLanguageProcessor = signLanguageProcessor;
        }
        [HttpPost("start")]
        public async Task<IActionResult> StartCall([FromBody] CallRequestDto request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var command = new StartCallCommand { CallerId = userId, ReceiverId = request.ReceiverId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("answer")]
        public async Task<IActionResult> AnswerCall([FromBody] AnswerRequestDto request)
        {
            var command = new AnswerCallCommand { CallId = request.CallId, Accept = request.Accept };
            await _mediator.Send(command);
            return Ok();
        }
        [HttpPost("end/{callId}")]
        public async Task<IActionResult> EndCall(string callId)
        {
            await _mediator.Send(new EndCallCommand { CallId = callId });
            return Ok();
        }
        [HttpGet("status/{callId}")]
        public async Task<IActionResult> GetCallStatus(string callId)
        {
            var result = await _mediator.Send(new GetCallStatusQuery { CallId = callId });
            return Ok(result);
        }
        [HttpPost("process-frame")]
        public async Task<IActionResult> ProcessFrame([FromBody] ProcessFrameRequest request)
        {
            var call = await _mediator.Send(new GetCallStatusQuery { CallId = request.CallId });
            if (call.Status != Domain.Enums.CallStatus.Active)
            {
                return BadRequest("Call is not active");
            }
            var text = await _signLanguageProcessor.ProcessFrameAsync(request.Frame);
            return Ok(new { text });
        }
    }
    public class ProcessFrameRequest
    {
        public string Frame { get; set; } // Base64-encoded image
        public string CallId { get; set; }
    }
}
