using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryAnnual
{
    public class GetEmployerFeedbackResultSummaryAnnualQueryResult
    {
        public List<EmployerFeedbackSummaryAnnualResult> AnnualEmployerFeedbackDetails { get; set; }
    }
}
