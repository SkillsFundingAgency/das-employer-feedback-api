using System;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public DateTime? CheckedOn { get; set; }
    }
}
