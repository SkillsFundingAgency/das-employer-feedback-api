using System;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class ProviderAttributeSummary
    {
        public long Ukprn { get; set; }
        public long AttributeId { get; set; } 
        public int Strength { get; set; }
        public int Weakness { get; set; }
        public string TimePeriod { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Attribute Attribute { get; set; }
    }
}