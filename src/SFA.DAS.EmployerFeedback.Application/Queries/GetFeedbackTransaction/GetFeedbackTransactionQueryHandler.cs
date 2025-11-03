using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction
{
    public class GetFeedbackTransactionQueryHandler : IRequestHandler<GetFeedbackTransactionQuery, GetFeedbackTransactionQueryResult>
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly ILogger<GetFeedbackTransactionQueryHandler> _logger;

        public GetFeedbackTransactionQueryHandler(
            IFeedbackTransactionContext feedbackTransactionContext,
            ILogger<GetFeedbackTransactionQueryHandler> logger)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _logger = logger;
        }

        public async Task<GetFeedbackTransactionQueryResult> Handle(GetFeedbackTransactionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting feedback transaction by Id: {Id}", request.Id);

            try
            {
                var feedbackTransaction = await _feedbackTransactionContext.GetByIdWithAccountAsync(request.Id, cancellationToken);

                if (feedbackTransaction == null)
                {
                    _logger.LogWarning("Feedback transaction with Id {Id} not found", request.Id);
                    return null;
                }

                var result = new GetFeedbackTransactionQueryResult
                {
                    Id = feedbackTransaction.Id,
                    AccountId = feedbackTransaction.AccountId,
                    AccountName = feedbackTransaction.Account?.AccountName,
                    TemplateName = feedbackTransaction.TemplateName,
                    TemplateId = feedbackTransaction.TemplateId,
                    CreatedOn = feedbackTransaction.CreatedOn,
                    SendAfter = feedbackTransaction.SendAfter,
                    SentCount = feedbackTransaction.SentCount,
                    SentDate = feedbackTransaction.SentDate
                };

                _logger.LogDebug("Successfully retrieved feedback transaction with Id: {Id}", request.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback transaction with Id: {Id}", request.Id);
                throw;
            }
        }
    }
}