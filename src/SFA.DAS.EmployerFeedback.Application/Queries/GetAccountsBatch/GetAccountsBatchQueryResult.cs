using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch
{
    public class GetAccountsBatchQueryResult
    {
        public List<long> AccountIds { get; set; } = new List<long>();
    }
}