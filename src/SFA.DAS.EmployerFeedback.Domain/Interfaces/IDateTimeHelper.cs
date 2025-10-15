using System;

namespace SFA.DAS.EmployerFeedback.Domain.Interfaces
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
    }

    public class UtcTimeProvider : IDateTimeHelper
    {
        public DateTime Now => DateTime.UtcNow;
    }
}