using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/employerfeedback/")]
    public class EmployerFeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployerFeedbackController> _logger;

        public EmployerFeedbackController(IMediator mediator, ILogger<EmployerFeedbackController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetLatestResults(long accountId, Guid userRef)
        {
            try
            {
                var result = await _mediator.Send(new GetLatestEmployerFeedbackResultsQuery { AccountId = accountId, UserRef = userRef });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error while retrieving latest employer feedback");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
