using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback
{
    public class GetAllEmployerFeedbackQueryResult
    {
        public List<AllEmployerFeedbackResults> Feedbacks { get; set; }
    }
}
