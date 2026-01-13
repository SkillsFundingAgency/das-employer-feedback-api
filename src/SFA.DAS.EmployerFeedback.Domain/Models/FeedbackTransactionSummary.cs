using System;

namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class FeedbackTransactionSummary
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public DateTime SendAfter { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
