using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultForAcademicYear
{
    public class GetEmployerFeedbackResultForAcademicYearQuery : IRequest<GetEmployerFeedbackResultForAcademicYearQueryResult>
    {
        public long Ukprn { get; set; }
        public string TimePeriod { get; set; }
    }
}