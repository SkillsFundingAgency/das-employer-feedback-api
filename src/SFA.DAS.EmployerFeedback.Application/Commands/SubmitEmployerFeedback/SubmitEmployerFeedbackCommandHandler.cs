using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback
{
    public class SubmitEmployerFeedbackCommandHandler : IRequestHandler<SubmitEmployerFeedbackCommand, SubmitEmployerFeedbackCommandResponse>
    {
        private readonly IEmployerFeedbackContext _employerFeedbackContext;
        private readonly IEmployerFeedbackResultContext _employerFeedbackResultContext;
        private readonly IProviderAttributeContext _providerAttributeContext;
        private readonly ILogger<SubmitEmployerFeedbackCommandHandler> _logger;

        public SubmitEmployerFeedbackCommandHandler(
            IEmployerFeedbackContext employerFeedbackContext,
            IEmployerFeedbackResultContext employerFeedbackResultContext,
            IProviderAttributeContext providerAttributeContext,
            ILogger<SubmitEmployerFeedbackCommandHandler> logger)
        {
            _employerFeedbackContext = employerFeedbackContext;
            _employerFeedbackResultContext = employerFeedbackResultContext;
            _providerAttributeContext = providerAttributeContext;
            _logger = logger;
        }

        public async Task<SubmitEmployerFeedbackCommandResponse> Handle(SubmitEmployerFeedbackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Upsert EmployerFeedback 
                var feedback = await _employerFeedbackContext.GetByUserUkprnAccountAsync(request.UserRef, request.Ukprn, request.AccountId, cancellationToken);
                if (feedback == null)
                {
                    feedback = new Domain.Entities.EmployerFeedback
                    {
                        UserRef = request.UserRef,
                        Ukprn = request.Ukprn,
                        AccountId = request.AccountId,
                        IsActive = true
                    };
                    _employerFeedbackContext.Add(feedback);
                }
                else if (!feedback.IsActive)
                {
                    feedback.IsActive = true;
                    _employerFeedbackContext.Update(feedback);
                }

                // Insert EmployerFeedbackResult
                var feedbackResult = new EmployerFeedbackResult
                {
                    EmployerFeedback = feedback,
                    ProviderRating = request.ProviderRating.ToString(),
                    FeedbackSource = request.FeedbackSource,
                    DateTimeCompleted = DateTime.UtcNow
                };
                _employerFeedbackResultContext.Add(feedbackResult);

                // Insert ProviderAttributes for the new EmployerFeedbackResult
                if (request.ProviderAttributes != null && request.ProviderAttributes.Any())
                {
                    foreach (var attr in request.ProviderAttributes)
                    {
                        var providerAttribute = new ProviderAttribute
                        {
                            EmployerFeedbackResult = feedbackResult,
                            AttributeId = attr.AttributeId,
                            AttributeValue = attr.AttributeValue
                        };
                        _providerAttributeContext.Add(providerAttribute);
                    }
                }

                await _employerFeedbackContext.SaveChangesAsync(cancellationToken);
                return new SubmitEmployerFeedbackCommandResponse() { EmployerFeedbackId = feedback.FeedbackId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting employer feedback");
                throw;
            }
        }
    }
}