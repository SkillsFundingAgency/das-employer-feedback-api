using System;
using System.Collections.Generic;
namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class EmployerFeedbackTarget
    {
        public long FeedbackId { get; set; }
        public Guid UserRef { get; set; }
        public long Ukprn { get; set; }
        public long AccountId { get; set; }
        public bool IsActive { get; set; }

        public Account Account { get; set; }

        public List<EmployerFeedbackResult> EmployerFeedbackResults { get; set; }
    }
}
