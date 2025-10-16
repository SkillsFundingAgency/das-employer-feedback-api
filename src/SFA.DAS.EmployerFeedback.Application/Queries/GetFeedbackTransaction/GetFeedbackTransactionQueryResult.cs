using System;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction
{
    public class GetFeedbackTransactionQueryResult
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public string TemplateName { get; set; }
        public Guid? TemplateId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? SendAfter { get; set; }
        public int? SentCount { get; set; }
        public DateTime? SentDate { get; set; }
    }
}