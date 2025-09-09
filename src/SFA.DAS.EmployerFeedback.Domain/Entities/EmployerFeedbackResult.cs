using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class EmployerFeedbackResult
    {
        public Guid Id { get; set; }
        public long FeedbackId { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public string ProviderRating { get; set; }
        public int FeedbackSource { get; set; }
        public EmployerFeedback EmployerFeedback { get; set; }
        public ICollection<ProviderAttribute> ProviderAttributes { get; set; }
    }
}