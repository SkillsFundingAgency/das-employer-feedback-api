using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Models
{
    public class UpsertFeedbackTransactionRequest
    {
        public List<ProviderCourseDto> Active { get; set; } = new List<ProviderCourseDto>();
        public List<ProviderCourseDto> Completed { get; set; } = new List<ProviderCourseDto>();
        public List<ProviderCourseDto> NewStart { get; set; } = new List<ProviderCourseDto>();
    }

    public class ProviderCourseDto
    {
        public long Ukprn { get; set; }
        public string CourseCode { get; set; }
    }
}