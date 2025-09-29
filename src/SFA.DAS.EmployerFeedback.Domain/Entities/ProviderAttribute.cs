using System;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class ProviderAttribute
    {
        public Guid EmployerFeedbackResultId { get; set; }
        public long AttributeId { get; set; }
        public int AttributeValue { get; set; }
        public EmployerFeedbackResult EmployerFeedbackResult { get; set; }
        public Attribute Attribute { get; set; } 
    }
}