using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQuery : IRequest<GetOverallEmployerFeedbackResultQueryResult>
    {
        public long Ukprn { get; set; }
    }
}