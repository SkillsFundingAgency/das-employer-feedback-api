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

            var starsSummary = starsSummaries.FirstOrDefault();

            if (starsSummary == null || attributeSummaries == null || !attributeSummaries.Any())
            {
                return new GetEmployerFeedbackResultSummaryForAcademicYearQueryResult
                {
                    AcademicYearEmployerFeedbackDetails = new EmployerFeedbackSummaryForAcademicYearResult
                    {
                        Ukprn = request.Ukprn,
                        TimePeriod = request.TimePeriod,
                        ProviderAttribute = new List<ProviderAttributeSummaryForAcademicYearResult>()
                    }
                };
            }

            var attributesForPeriod = attributeSummaries
                .Where(a => a.Ukprn == starsSummary.Ukprn && a.TimePeriod == starsSummary.TimePeriod && a.Attribute?.AttributeName != null)
                .Select(a => new ProviderAttributeSummaryForAcademicYearResult
                {
                    Name = a.Attribute.AttributeName,
                    Strength = a.Strength,
                    Weakness = a.Weakness
                })
                .ToList();

            return new GetEmployerFeedbackResultSummaryForAcademicYearQueryResult
            {
                AcademicYearEmployerFeedbackDetails = new EmployerFeedbackSummaryForAcademicYearResult
                {
                    Ukprn = starsSummary.Ukprn,
                    Stars = starsSummary.Stars,
                    ReviewCount = starsSummary.ReviewCount,
                    TimePeriod = starsSummary.TimePeriod,
                    ProviderAttribute = attributesForPeriod
                }
            };
        }
    }
}