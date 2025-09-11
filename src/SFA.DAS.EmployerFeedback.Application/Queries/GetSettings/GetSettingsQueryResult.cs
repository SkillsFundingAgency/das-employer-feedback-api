using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetSettings
{
    public class GetSettingsQueryResult
    {
        public List<Settings> Settings { get; set; }
    }

    public class Settings
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
