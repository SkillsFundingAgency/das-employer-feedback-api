using System;

namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class LatestEmployerFeedbackResults
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long Ukprn { get; set; }
        public DateTime? DateTimeCompleted { get; set;  }
        public string ProviderRating { get; set; }
        public int? FeedbackSource { get; set; }
    }
}
