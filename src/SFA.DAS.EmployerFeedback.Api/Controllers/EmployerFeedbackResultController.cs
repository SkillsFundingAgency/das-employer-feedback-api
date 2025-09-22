using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummary;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryAnnual;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryForAcademicYear;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/employerfeedbackresult")]
    public class EmployerFeedbackResultController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployerFeedbackResultController> _logger;

        public EmployerFeedbackResultController(IMediator mediator, ILogger<EmployerFeedbackResultController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> SubmitEmployerFeedback([FromBody] SubmitEmployerFeedbackRequest request)
        {
            try
            {
                var command = (SubmitEmployerFeedbackCommand)request;
                await _mediator.Send(command);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for employer feedback submission");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error submitting employer feedback");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        [HttpGet("{ukprn}")]
        public async Task<IActionResult> GetEmployerFeedbackResultSummary(long ukprn)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployerFeedbackResultSummaryQuery { Ukprn = ukprn });
                return Ok(result.EmployerFeedbackResultSummary);
            }
            catch (Exception e)
            {
                var message = $"Unhandled error when attempting to get employer feedback result summary for UKPRN {ukprn}: {e.Message}";
                _logger.LogError(message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        [HttpGet("{ukprn}/annual")]
        public async Task<IActionResult> GetEmployerFeedbackResultAnnual(long ukprn)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployerFeedbackResultSummaryAnnualQuery { Ukprn = ukprn });

                return Ok(result);
            }
            catch (Exception e)
            {
                var message = $"Unhandled error when attempting to get employer feedback result annual for UKPRN {ukprn}: {e.Message}";
                _logger.LogError(message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        [HttpGet("{ukprn}/annual/{year}")]
        public async Task<IActionResult> GetEmployerFeedbackResultForAcademicYear(long ukprn, string year)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployerFeedbackResultSummaryForAcademicYearQuery { Ukprn = ukprn, TimePeriod =year});

                return Ok(result.AcademicYearEmployerFeedbackDetails);
            }
            catch (Exception e)
            {
                var message = $"Unhandled error when attempting to get employer feedback result for academic year for UKPRN {ukprn}: {e.Message}";
                _logger.LogError(message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}