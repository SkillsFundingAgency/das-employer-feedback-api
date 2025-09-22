using ESFA.DAS.ProvideFeedback.Data.Constants;
using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummary
{
    public class GetEmployerFeedbackResultSummaryQueryHandler : IRequestHandler<GetEmployerFeedbackResultSummaryQuery, GetEmployerFeedbackResultSummaryQueryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;

        public GetEmployerFeedbackResultSummaryQueryHandler(
            IProviderStarsSummaryContext providerStarsSummaryContext,
            IProviderAttributeSummaryContext providerAttributeSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
        }

        public async Task<GetEmployerFeedbackResultSummaryQueryResult> Handle(
            GetEmployerFeedbackResultSummaryQuery request,
            CancellationToken cancellationToken)
        {

            var attributeSummaries = await _providerAttributeSummaryContext
                .GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(request.Ukprn, ReviewDataPeriod.All, cancellationToken);

            var starsSummaries = await _providerStarsSummaryContext
                .GetProviderStarsSummaryByUkprnAndTimePeriodAsync(request.Ukprn, ReviewDataPeriod.All, cancellationToken);


            var starsSummary = starsSummaries.FirstOrDefault();

            if (starsSummary == null || attributeSummaries == null || !attributeSummaries.Any())
            {
                return new GetEmployerFeedbackResultSummaryQueryResult
                {
                    EmployerFeedbackResultSummary = new EmployerFeedbackSummaryResult
                    {
                        Ukprn = request.Ukprn,
                        ProviderAttribute = new List<ProviderAttributeSummaryResult>()
                    }
                };
            }

            var providerAttribute = attributeSummaries.Select(x => new ProviderAttributeSummaryResult
            {
                Name = x.Attribute.AttributeName,
                Strength = x.Strength,
                Weakness = x.Weakness
            }).ToList();

            return new GetEmployerFeedbackResultSummaryQueryResult
            {
                EmployerFeedbackResultSummary = new EmployerFeedbackSummaryResult
                {
                    Ukprn = starsSummary.Ukprn,
                    Stars = starsSummary.Stars,
                    ReviewCount = starsSummary.ReviewCount,
                    TimePeriod = starsSummary.TimePeriod,
                    ProviderAttribute = providerAttribute
                }
            };
        }
    }
}