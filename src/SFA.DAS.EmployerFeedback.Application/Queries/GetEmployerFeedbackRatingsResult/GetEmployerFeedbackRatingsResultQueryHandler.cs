using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryHandler : IRequestHandler<GetEmployerFeedbackRatingsResultQuery, GetEmployerFeedbackRatingsResultQueryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetEmployerFeedbackRatingsResultQueryHandler(
            IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetEmployerFeedbackRatingsResultQueryResult> Handle(
            GetEmployerFeedbackRatingsResultQuery request,
            CancellationToken cancellationToken)
        {
            var starsSummaries = await _providerStarsSummaryContext
                .GetProviderStarsSummaryByTimePeriodAsync(request.TimePeriod, cancellationToken);

            var results = starsSummaries?
           .Select(x => new EmployerFeedbackRatingsResult
           {
               Ukprn = x.Ukprn,
               ReviewCount = x.ReviewCount,
               Stars = x.Stars
           })
           .ToList() ?? new List<EmployerFeedbackRatingsResult>();

            return new GetEmployerFeedbackRatingsResultQueryResult
            {
                EmployerFeedbackRatings = results
            };
        }
    }
}