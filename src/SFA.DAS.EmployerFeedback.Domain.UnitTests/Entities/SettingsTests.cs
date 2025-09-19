using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class SettingsTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var id = Guid.NewGuid();
            var name = "TestSetting";
            var value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var settings = new Settings
            {
                Id = id,
                Name = name,
                Value = value
            };
            settings.Id.Should().Be(id);
            settings.Name.Should().Be(name);
            settings.Value.Should().Be(value);
        }
    }
}
