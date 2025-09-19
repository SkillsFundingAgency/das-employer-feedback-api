using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class ProviderAttributeResults
    {
        public string Name { get; set; }
        public int? Value { get; set; }
    }

    public class AllEmployerFeedbackResults
    {
        public long Ukprn { get; set; }
        public DateTime DateTimeCompleted { get; set; }
        public string ProviderRating { get; set; }
        public List<ProviderAttributeResults> ProviderAttributes { get; set; }
    }
}
