using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackStarsResultAnnual
{
    public class GetEmployerFeedbackStarsResultAnnualQueryHandler : IRequestHandler<GetEmployerFeedbackStarsResultAnnualQuery, GetEmployerFeedbackStarsResultAnnualQueryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetEmployerFeedbackStarsResultAnnualQueryHandler(
            IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetEmployerFeedbackStarsResultAnnualQueryResult> Handle(
            GetEmployerFeedbackStarsResultAnnualQuery request,
            CancellationToken cancellationToken)
        {
            var starsSummaries = await _providerStarsSummaryContext
                .GetProviderStarsSummaryByTimePeriodAsync(request.TimePeriod, cancellationToken);

            var results = starsSummaries?
           .Select(x => new GetEmployerFeedbackStarsAnnualResult
           {
               Ukprn = x.Ukprn,
               ReviewCount = x.ReviewCount,
               Stars = x.Stars
           })
           .ToList() ?? new List<GetEmployerFeedbackStarsAnnualResult>();

            return new GetEmployerFeedbackStarsResultAnnualQueryResult
            {
                AnnualEmployerFeedbackStarsDetails = results
            };
        }
    }
}