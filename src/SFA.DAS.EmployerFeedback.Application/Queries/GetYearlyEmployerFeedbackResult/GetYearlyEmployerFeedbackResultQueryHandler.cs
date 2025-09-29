using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQueryHandler : IRequestHandler<GetYearlyEmployerFeedbackResultQuery, GetYearlyEmployerFeedbackResultQueryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;

        public GetYearlyEmployerFeedbackResultQueryHandler(
            IProviderStarsSummaryContext providerStarsSummaryContext,
            IProviderAttributeSummaryContext providerAttributeSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
        }

        public async Task<GetYearlyEmployerFeedbackResultQueryResult> Handle(
            GetYearlyEmployerFeedbackResultQuery request,
            CancellationToken cancellationToken)
        {
            var starsSummaries = await _providerStarsSummaryContext
                .GetProviderStarsSummaryByUkprnAsync(request.Ukprn, cancellationToken);

            var attributeSummaries = await _providerAttributeSummaryContext
                .GetProviderAttributeSummaryByUkprnAsync(request.Ukprn, cancellationToken);

            var result = new List<Domain.Models.GetYearlyEmployerFeedbackResult>();

            foreach (var stars in starsSummaries)
            {
                var attributesForPeriod = attributeSummaries
                    .Where(a => a.Ukprn == stars.Ukprn && a.TimePeriod == stars.TimePeriod && a.Attribute?.AttributeName != null)
                    .Select(a => new GetYearlyEmployerFeedbackResultProviderAttribute
                    {
                        Name = a.Attribute.AttributeName,
                        Strength = a.Strength,
                        Weakness = a.Weakness
                    })
                    .ToList();

                if (attributesForPeriod.Any())
                {
                    result.Add(new Domain.Models.GetYearlyEmployerFeedbackResult
                    {
                        Ukprn = stars.Ukprn,
                        Stars = stars.Stars,
                        ReviewCount = stars.ReviewCount,
                        TimePeriod = stars.TimePeriod,
                        ProviderAttribute = attributesForPeriod
                    });
                }
            }

            return new GetYearlyEmployerFeedbackResultQueryResult
            {
                AnnualEmployerFeedbackDetails = result
            };
        }
    }
}