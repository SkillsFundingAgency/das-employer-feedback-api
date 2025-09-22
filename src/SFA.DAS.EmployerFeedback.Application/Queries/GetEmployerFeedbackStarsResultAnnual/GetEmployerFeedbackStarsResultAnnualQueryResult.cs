using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackStarsResultAnnual
{
    public class GetEmployerFeedbackStarsResultAnnualQueryResult
    {
        public List<GetEmployerFeedbackStarsAnnualResult> AnnualEmployerFeedbackStarsDetails { get; set; }
    }
}
