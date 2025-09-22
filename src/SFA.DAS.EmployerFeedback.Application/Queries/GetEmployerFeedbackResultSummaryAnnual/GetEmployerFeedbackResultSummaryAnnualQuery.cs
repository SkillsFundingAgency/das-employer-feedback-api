using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryAnnual
{
    public class GetEmployerFeedbackResultSummaryAnnualQuery : IRequest<GetEmployerFeedbackResultSummaryAnnualQueryResult>
    {
        public long Ukprn { get; set; }
    }
}