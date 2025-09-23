using ESFA.DAS.ProvideFeedback.Data.Constants;
using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQueryHandler : IRequestHandler<GetOverallEmployerFeedbackResultQuery, GetOverallEmployerFeedbackResultQueryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;

        public GetOverallEmployerFeedbackResultQueryHandler(
            IProviderStarsSummaryContext providerStarsSummaryContext,
            IProviderAttributeSummaryContext providerAttributeSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
        }

        public async Task<GetOverallEmployerFeedbackResultQueryResult> Handle(
            GetOverallEmployerFeedbackResultQuery request,
            CancellationToken cancellationToken)
        {

            var attributeSummaries = await _providerAttributeSummaryContext
                .GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(request.Ukprn, ReviewDataPeriod.All, cancellationToken);

            var starsSummaries = await _providerStarsSummaryContext
                .GetProviderStarsSummaryByUkprnAndTimePeriodAsync(request.Ukprn, ReviewDataPeriod.All, cancellationToken);


            var starsSummary = starsSummaries.FirstOrDefault();

            if (starsSummary == null || attributeSummaries == null || !attributeSummaries.Any())
            {
                return new GetOverallEmployerFeedbackResultQueryResult
                {
                    OverallEmployerFeedback = new OverallEmployerFeedbackResult
                    {
                        Ukprn = request.Ukprn,
                        ProviderAttribute = new List<OverallEmployerFeedbackResultProviderAttribute>()
                    }
                };
            }

            var providerAttribute = attributeSummaries.Select(x => new OverallEmployerFeedbackResultProviderAttribute
            {
                Name = x.Attribute.AttributeName,
                Strength = x.Strength,
                Weakness = x.Weakness
            }).ToList();

            return new GetOverallEmployerFeedbackResultQueryResult
            {
                OverallEmployerFeedback = new OverallEmployerFeedbackResult
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