using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountsBatch([FromQuery] int batchsize)
        {
            try
            {
                if (batchsize <= 0)
                {
                    return BadRequest("Batch size must be greater than zero.");
                }

                var query = new GetAccountsBatchQuery { BatchSize = batchsize };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving next account batch");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpsertAccounts([FromBody] UpsertAccountsRequest request)
        {
            try
            {
                var command = (UpsertAccountsCommand)request;
                await _mediator.Send(command);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for accounts upsert");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting accounts");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("{id}/feedbacktransaction")]
        public async Task<IActionResult> UpsertFeedbackTransaction([FromRoute] long id, [FromBody] UpsertFeedbackTransactionRequest request)
        {
            try
            {
                var command = (UpsertFeedbackTransactionCommand)request;
                command.AccountId = id;

                await _mediator.Send(command);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for feedback transaction upsert for AccountId: {AccountId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting feedback transaction for AccountId: {AccountId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
