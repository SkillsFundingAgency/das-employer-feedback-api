using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackStarsResultAnnual
{
    public class GetEmployerFeedbackStarsResultAnnualQuery : IRequest<GetEmployerFeedbackStarsResultAnnualQueryResult>
    {
        public string TimePeriod { get; set; }
    }
}