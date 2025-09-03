using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults
{
    public class GetLatestEmployerFeedbackResultsQueryHandler : IRequestHandler<GetLatestEmployerFeedbackResultsQuery, GetLatestEmployerFeedbackResultsQueryResult>
    {
        private readonly IEmployerFeedbackTargetContext _entityContext;

        public GetLatestEmployerFeedbackResultsQueryHandler(IEmployerFeedbackTargetContext entityContext)
        {
            _entityContext = entityContext;
        }

        public async Task<GetLatestEmployerFeedbackResultsQueryResult> Handle(GetLatestEmployerFeedbackResultsQuery request, CancellationToken cancellationToken)
        {
            var results = await _entityContext.GetLatestResultsPerAccount(request.AccountId, request.UserRef, cancellationToken);
            
            var first = results.FirstOrDefault();
            if (first == null)
                return null;

            return new GetLatestEmployerFeedbackResultsQueryResult
            {
                AccountId = first.AccountId,
                AccountName = first.AccountName,
                EmployerFeedbacks = results.Select(x => 
                    new EmployerFeedbackItem
                    { 
                        Ukprn = x.Ukprn, 
                        Result = x.DateTimeCompleted != null ? new FeedbackResultItem 
                        { 
                            DateTimeCompleted = x.DateTimeCompleted.Value, 
                            ProviderRating = x.ProviderRating, 
                            FeedbackSource = x.FeedbackSource.Value 
                        } : null
                    }).ToList()
            };
        }
    }
}
