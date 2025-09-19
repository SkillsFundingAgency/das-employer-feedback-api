using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertRefreshALELastRunDateSetting;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetRefreshALELastRunDateSetting;

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

        [HttpGet("RefreshALELastRunDate")]
        public async Task<IActionResult> GetRefreshALELastRunDateSetting()
        {
            try
            {
                var result = await _mediator.Send(new GetRefreshALELastRunDateSettingQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving setting");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("RefreshALELastRunDate")]
        public async Task<IActionResult> UpsertRefreshALELastRunDateSetting([FromBody] SettingRequest setting)
        {
            try
            {
                var command = new UpsertRefreshALELastRunDateSettingCommand
                {
                    Value = setting.Value
                };
                await _mediator.Send(command);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for setting upsert");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving setting");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
