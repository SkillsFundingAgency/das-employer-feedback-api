using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetSettings;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IMediator mediator, ILogger<SettingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            try
            {
                var result = await _mediator.Send(new GetSettingsQuery());
                return Ok(result.Settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("RefreshALELastRunDate")]
        public async Task<IActionResult> UpsertSettings([FromBody] SettingRequest setting)
        {
            try
            {
                var command = new UpsertSettingsCommand
                {
                    Value = setting.Value
                };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for settings upsert");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
