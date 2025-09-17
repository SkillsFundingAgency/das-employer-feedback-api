using System;

namespace SFA.DAS.EmployerFeedback.Domain.Entities
{
    public class Settings
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? Value { get; set; }
    }
}
