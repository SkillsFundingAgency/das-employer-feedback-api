using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummary
{
    public class GetEmployerFeedbackResultSummaryQuery : IRequest<GetEmployerFeedbackResultSummaryQueryResult>
    {
        public long Ukprn { get; set; }
    }
}