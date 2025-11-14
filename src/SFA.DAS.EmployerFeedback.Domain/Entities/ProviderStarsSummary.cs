
namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class ProviderStarsSummary
    {
        public long Ukprn { get; set; }
        public int ReviewCount { get; set; }
        public int Stars { get; set; }
        public string TimePeriod { get; set; }
    }
}
