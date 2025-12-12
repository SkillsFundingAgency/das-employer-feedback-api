using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction
{
    public class GetFeedbackTransactionQuery : IRequest<GetFeedbackTransactionQueryResult>
    {
        public long Id { get; set; }
    }
}