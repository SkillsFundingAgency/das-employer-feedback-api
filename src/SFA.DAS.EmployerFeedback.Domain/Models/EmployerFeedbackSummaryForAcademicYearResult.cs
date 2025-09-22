using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class EmployerFeedbackSummaryForAcademicYearResult
    {
        public long Ukprn { get; set; }

        public int Stars { get; set; }

        public int ReviewCount { get; set; }

        public string TimePeriod { get; set; }

        public List<ProviderAttributeSummaryForAcademicYearResult> ProviderAttribute { get; set; }
    }
    public class ProviderAttributeSummaryForAcademicYearResult
    {
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Weakness { get; set; }
    }
}