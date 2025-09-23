using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryResult
    {
        public List<EmployerFeedbackRatingsResult> EmployerFeedbackRatings { get; set; }
    }
}
