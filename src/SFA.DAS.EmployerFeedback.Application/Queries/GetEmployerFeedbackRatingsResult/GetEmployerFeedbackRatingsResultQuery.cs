using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQuery : IRequest<GetEmployerFeedbackRatingsResultQueryResult>
    {
        public string TimePeriod { get; set; }
    }
}