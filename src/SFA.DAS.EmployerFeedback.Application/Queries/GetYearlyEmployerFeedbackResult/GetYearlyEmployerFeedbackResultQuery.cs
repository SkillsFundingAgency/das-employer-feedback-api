using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQuery : IRequest<GetYearlyEmployerFeedbackResultQueryResult>
    {
        public long Ukprn { get; set; }
    }
}