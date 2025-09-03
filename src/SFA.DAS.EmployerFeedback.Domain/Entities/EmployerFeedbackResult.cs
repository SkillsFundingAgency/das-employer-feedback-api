using System;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class EmployerFeedbackResult
    {
        public Guid Id { get; set; }
        public long FeedbackId { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public string ProviderRating { get; set; } = null!;
        public int? FeedbackSource { get; set; }

        public EmployerFeedbackTarget EmployerFeedbackTarget { get; set; }
    }
}
