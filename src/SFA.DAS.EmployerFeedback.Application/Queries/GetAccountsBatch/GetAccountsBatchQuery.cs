using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch
{
    public class GetAccountsBatchQuery : IRequest<GetAccountsBatchQueryResult>
    {
        public int BatchSize { get; set; }
    }
}