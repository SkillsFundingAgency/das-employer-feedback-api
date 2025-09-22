
namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class GetEmployerFeedbackStarsAnnualResult
    {
        public long Ukprn { get; set; }

        public int Stars { get; set; }

        public int ReviewCount { get; set; }
    }
}