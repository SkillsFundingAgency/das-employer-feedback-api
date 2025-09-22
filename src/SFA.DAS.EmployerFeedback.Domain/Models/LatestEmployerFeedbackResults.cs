using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class LatestEmployerFeedbackResults
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
        public long Ukprn { get; set; }
        public DateTime? DateTimeCompleted { get; set;  }
        public string ProviderRating { get; set; }
        public int? FeedbackSource { get; set; }
    }
}
