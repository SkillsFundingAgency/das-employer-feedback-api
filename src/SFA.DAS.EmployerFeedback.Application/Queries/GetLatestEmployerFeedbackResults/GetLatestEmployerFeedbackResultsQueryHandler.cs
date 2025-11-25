using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults
{
    public class GetLatestEmployerFeedbackResultsQueryHandler : IRequestHandler<GetLatestEmployerFeedbackResultsQuery, GetLatestEmployerFeedbackResultsQueryResult>
    {
        private readonly IAccountContext _accountContext;

        public GetLatestEmployerFeedbackResultsQueryHandler(IAccountContext accountContext)
        {
            _accountContext = accountContext;
        }

        public async Task<GetLatestEmployerFeedbackResultsQueryResult> Handle(GetLatestEmployerFeedbackResultsQuery request, CancellationToken cancellationToken)
        {
            var results = await _accountContext.GetLatestResultsPerAccount(
                request.AccountId, request.UserRef, cancellationToken);

            var first = results.FirstOrDefault();
            if (first == null)
                return null;

            var items = results
                .Where(x => x.Ukprn.HasValue && x.DateTimeCompleted != null)
                .Select(x => new EmployerFeedbackItem
                {
                    Ukprn = x.Ukprn.Value,
                    Result = new FeedbackResultItem
                    {
                        DateTimeCompleted = x.DateTimeCompleted!.Value,
                        ProviderRating = x.ProviderRating,
                        FeedbackSource = x.FeedbackSource!.Value
                    }
                })
                .ToList();

            return new GetLatestEmployerFeedbackResultsQueryResult
            {
                AccountId = first.AccountId,
                AccountName = first.AccountName,
                EmployerFeedbacks = items.Count == 0 ? null : items
            };
        }
    }
}
