using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmailNudgeAccountsBatch
{
    public class GetEmailNudgeAccountsBatchQueryResult
    {
        public List<long> AccountIds { get; set; } = new List<long>();
    }
}