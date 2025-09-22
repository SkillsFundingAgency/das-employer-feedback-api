using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class EmployerFeedbackSummaryAnnualResult
    {
        public long Ukprn { get; set; }

        public int Stars { get; set; }

        public int ReviewCount { get; set; }

        public string TimePeriod { get; set; }

        public List<ProviderAttributeSummaryAnnualResult> ProviderAttribute { get; set; }
    }
    public class ProviderAttributeSummaryAnnualResult
    {
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Weakness { get; set; }
    }
}