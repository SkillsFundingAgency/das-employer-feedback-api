using SFA.DAS.EmployerFeedback.Application.Queries.GetSettings;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetSettings
{
    public class GetSettingsQueryResultTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(List<Settings> settings)
        {
            var result = new GetSettingsQueryResult { Settings = settings };
            result.Settings.Should().BeEquivalentTo(settings);
        }
    }
}
