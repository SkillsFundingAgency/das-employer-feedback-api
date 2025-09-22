using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryForAcademicYear
{
    public class GetEmployerFeedbackResultSummaryForAcademicYearQueryHandler : IRequestHandler<GetEmployerFeedbackResultSummaryForAcademicYearQuery, GetEmployerFeedbackResultSummaryForAcademicYearQueryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;

        public GetEmployerFeedbackResultSummaryForAcademicYearQueryHandler(
            IProviderStarsSummaryContext providerStarsSummaryContext,
            IProviderAttributeSummaryContext providerAttributeSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
        }

        public async Task<GetEmployerFeedbackResultSummaryForAcademicYearQueryResult> Handle(
            GetEmployerFeedbackResultSummaryForAcademicYearQuery request,
            CancellationToken cancellationToken)
        {
            var starsSummaries = await _providerStarsSummaryContext
                .GetProviderStarsSummaryByUkprnAndTimePeriodAsync(request.Ukprn, request.TimePeriod, cancellationToken);

            var attributeSummaries = await _providerAttributeSummaryContext
                .GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(request.Ukprn, request.TimePeriod, cancellationToken);

            var result = new List<EmployerFeedbackSummaryForAcademicYearResult>();

            foreach (var stars in starsSummaries)
            {
                var attributesForPeriod = attributeSummaries
                    .Where(a => a.Ukprn == stars.Ukprn && a.TimePeriod == stars.TimePeriod && a.Attribute?.AttributeName != null)
                    .Select(a => new ProviderAttributeSummaryForAcademicYearResult
                    {
                        Name = a.Attribute.AttributeName,
                        Strength = a.Strength,
                        Weakness = a.Weakness
                    })
                    .ToList();

                if (attributesForPeriod.Any())
                {
                    result.Add(new EmployerFeedbackSummaryForAcademicYearResult
                    {
                        Ukprn = stars.Ukprn,
                        Stars = stars.Stars,
                        ReviewCount = stars.ReviewCount,
                        TimePeriod = stars.TimePeriod,
                        ProviderAttribute = attributesForPeriod
                    });
                }
            }

            return new GetEmployerFeedbackResultSummaryForAcademicYearQueryResult
            {
                AcademicYearEmployerFeedbackDetails = result
            };
        }
    }
}