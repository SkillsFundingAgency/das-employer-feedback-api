using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultSummaryForAcademicYear
{
    public class GetEmployerFeedbackResultSummaryForAcademicYearQueryResult
    {
        public List<EmployerFeedbackSummaryForAcademicYearResult> AcademicYearEmployerFeedbackDetails { get; set; }
    }
}
