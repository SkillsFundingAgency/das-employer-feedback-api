using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/feedbacktransactions")]
    public class FeedbackTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FeedbackTransactionsController> _logger;

        public FeedbackTransactionsController(IMediator mediator, ILogger<FeedbackTransactionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeedbackTransactionsBatch([FromQuery] int batchsize)
        {
            try
            {
                if (batchsize <= 0)
                {
                    return BadRequest("Batch size must be greater than zero.");
                }

                var query = new GetFeedbackTransactionsBatchQuery { BatchSize = batchsize };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback transactions batch");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}