using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/attributes/")]
    public class AttributesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AttributesController> _logger;

        public AttributesController(IMediator mediator, ILogger<AttributesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAttributes()
        {
            try
            {
                _logger.LogError("GetAttributes called");
                var result = await _mediator.Send(new GetAttributesQuery());
                return Ok(result.Attributes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Server error while retrieving attributes");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
