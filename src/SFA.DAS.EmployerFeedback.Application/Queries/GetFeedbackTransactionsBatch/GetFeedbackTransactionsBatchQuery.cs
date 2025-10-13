using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch
{
    public class GetFeedbackTransactionsBatchQuery : IRequest<GetFeedbackTransactionsBatchQueryResult>
    {
        public int BatchSize { get; set; }
    }
}