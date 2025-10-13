using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch
{
    public class GetFeedbackTransactionsBatchQueryResult
    {
        public List<long> FeedbackTransactions { get; set; } = new List<long>();
    }
}