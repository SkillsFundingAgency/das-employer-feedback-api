using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackTransaction(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest($"Id {id} unknown");
                }

                var query = new GetFeedbackTransactionQuery { Id = id };
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return BadRequest($"Id {id} unknown");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback transaction with Id: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedbackTransaction([FromRoute] long id, [FromBody] UpdateFeedbackTransactionRequest request)
        {
            try
            {
                var command = (UpdateFeedbackTransactionCommand)request;
                command.Id = id;

                await _mediator.Send(command);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for feedback transaction update for Id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "FeedbackTransaction not found for Id: {Id}", id);
                return BadRequest($"FeedbackTransaction with id {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback transaction for Id: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}