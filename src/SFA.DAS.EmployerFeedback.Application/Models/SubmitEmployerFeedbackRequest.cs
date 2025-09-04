using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Models
{
    public class SubmitEmployerFeedbackRequest
    {
        public Guid UserRef { get; set; }
        public long Ukprn { get; set; }
        public long AccountId { get; set; }
        public OverallRating ProviderRating { get; set; }
        public int FeedbackSource { get; set; }
        public List<ProviderAttributeDto> ProviderAttributes { get; set; }
    }
}
