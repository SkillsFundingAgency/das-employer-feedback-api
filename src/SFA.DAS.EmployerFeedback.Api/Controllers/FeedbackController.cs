namespace ESFA.DAS.EmployerProvideFeedback.Api.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly ILogger<FeedbackController> _logger;
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator, ILogger<FeedbackController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _mediator.Send(new GetAllEmployerFeedbackQuery());
                if (result.Feedbacks.Any())
                {
                    return Ok(result.Feedbacks);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error while retrieving employer feedback ");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}