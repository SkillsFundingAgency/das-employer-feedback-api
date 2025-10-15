using MediatR;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmailNudgeAccountsBatch
{
    public class GetEmailNudgeAccountsBatchQuery : IRequest<GetEmailNudgeAccountsBatchQueryResult>
    {
        public int BatchSize { get; set; }
    }
}