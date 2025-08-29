using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Commands.GenerateFeedbackSummaries
{
    public class GenerateFeedbackSummariesCommandHandler : IRequestHandler<GenerateFeedbackSummariesCommand>
    {
        private readonly IProviderRatingSummaryContext _providerRatingSummaryContext;
        private readonly ILogger<GenerateFeedbackSummariesCommandHandler> _logger;

        public GenerateFeedbackSummariesCommandHandler(
            IProviderRatingSummaryContext providerRatingSummaryContext,
            ILogger<GenerateFeedbackSummariesCommandHandler> logger)
        {
            _providerRatingSummaryContext = providerRatingSummaryContext;
            _logger = logger;
        }

        public async Task Handle(GenerateFeedbackSummariesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Begin Generation of Feedback Summaries in Handler: {DateTime.UtcNow}");
            await _providerRatingSummaryContext.GenerateFeedbackSummaries();
            _logger.LogInformation($"Successfully Generated Feedback Summaries in Handler: {DateTime.UtcNow}");
        }
    }
}