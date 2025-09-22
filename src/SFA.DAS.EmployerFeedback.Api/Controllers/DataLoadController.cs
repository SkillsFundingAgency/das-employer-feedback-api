using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using SFA.DAS.EmployerFeedback.Api.TaskQueue;
using SFA.DAS.EmployerFeedback.Application.Commands.GenerateFeedbackSummaries;
using SFA.DAS.EmployerFeedback.Application.Extensions;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class DataLoadController : Controller
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly ILogger<DataLoadController> _logger;

        public DataLoadController(IBackgroundTaskQueue backgroundTaskQueue, ILogger<DataLoadController> logger)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
        }

        [HttpPost("generate-feedback-summaries")]
        public IActionResult GenerateFeedbackSummaries()
        {
            var requestName = "generate feedback summaries";

            try
            {
                _logger.LogInformation($"Received request to {requestName}");

                _backgroundTaskQueue.QueueBackgroundRequest(
                    new GenerateFeedbackSummariesCommand(), requestName, (response, duration, log) =>
                    {
                        log.LogInformation($"Completed request to {requestName}: Request completed in {duration.ToReadableString()}");
                    });

                _logger.LogInformation($"Queued request to {requestName}");

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to {requestName}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error {requestName}: {e.Message}");
            }
        }
    }
}
