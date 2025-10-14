using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch
{
    public class GetFeedbackTransactionsBatchQueryHandler : IRequestHandler<GetFeedbackTransactionsBatchQuery, GetFeedbackTransactionsBatchQueryResult>
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public GetFeedbackTransactionsBatchQueryHandler(IFeedbackTransactionContext feedbackTransactionContext, IDateTimeHelper dateTimeHelper)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<GetFeedbackTransactionsBatchQueryResult> Handle(GetFeedbackTransactionsBatchQuery request, CancellationToken cancellationToken)
        {
            var feedbackTransactionIds = await _feedbackTransactionContext.GetFeedbackTransactionsBatchAsync(request.BatchSize, _dateTimeHelper.Now, cancellationToken);

            return new GetFeedbackTransactionsBatchQueryResult
            {
                FeedbackTransactions = feedbackTransactionIds
            };
        }
    }
}