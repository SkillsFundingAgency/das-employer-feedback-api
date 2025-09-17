using SFA.DAS.EmployerFeedback.Application.Queries.GetSettings;
using NUnit.Framework;
using System;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetSettings
{
    public class GetSettingsQueryResultTests
    {
        [Test]
        public void CanInstantiate_GetSettingsQueryResult()
        {
            var now = DateTime.UtcNow;
            var result = new GetSettingsQueryResult { Value = now };
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(now));
        }
    }
}
