using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch
{
    public class GetFeedbackTransactionsBatchQueryHandler : IRequestHandler<GetFeedbackTransactionsBatchQuery, GetFeedbackTransactionsBatchQueryResult>
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;

        public GetFeedbackTransactionsBatchQueryHandler(IFeedbackTransactionContext feedbackTransactionContext)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
        }

        public async Task<GetFeedbackTransactionsBatchQueryResult> Handle(GetFeedbackTransactionsBatchQuery request, CancellationToken cancellationToken)
        {
            var feedbackTransactionIds = await _feedbackTransactionContext.GetFeedbackTransactionsBatchAsync(request.BatchSize, cancellationToken);

            return new GetFeedbackTransactionsBatchQueryResult
            {
                FeedbackTransactions = feedbackTransactionIds
            };
        }
    }
}