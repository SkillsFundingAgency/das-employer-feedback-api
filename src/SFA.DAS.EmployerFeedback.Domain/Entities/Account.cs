using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? CheckedOn { get; set; }

        public ICollection<EmployerFeedback> EmployerFeedbacks { get; set; }
    }
}
