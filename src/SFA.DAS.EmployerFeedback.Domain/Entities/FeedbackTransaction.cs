using System;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class FeedbackTransaction
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string TemplateName { get; set; }
        public DateTime SendAfter { get; set; }
        public Guid? TemplateId { get; set; }
        public int? SentCount { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime CreatedOn { get; set; }

        public Account Account { get; set; }
    }
}