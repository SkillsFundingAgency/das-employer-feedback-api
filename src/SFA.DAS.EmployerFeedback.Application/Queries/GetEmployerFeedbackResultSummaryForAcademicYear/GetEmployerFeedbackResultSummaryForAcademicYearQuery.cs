using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryForAcademicYear
{
    public class GetEmployerFeedbackResultSummaryForAcademicYearQuery : IRequest<GetEmployerFeedbackResultSummaryForAcademicYearQueryResult>
    {
        public long Ukprn { get; set; }
        public string TimePeriod { get; set; }
    }
}