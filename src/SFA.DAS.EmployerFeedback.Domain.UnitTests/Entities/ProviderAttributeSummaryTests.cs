using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class ProviderAttributeSummaryTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var attr = new Domain.Entities.Attribute { AttributeId = 1, AttributeName = "test" };
            var summary = new ProviderAttributeSummary
            {
                Ukprn = 12345678,
                AttributeId = 1,
                Strength = 2,
                Weakness = 1,
                TimePeriod = "AY2023",
                UpdatedOn = new DateTime(2024, 1, 1),
                Attribute = attr
            };
            Assert.That(summary.Ukprn, Is.EqualTo(12345678));
            Assert.That(summary.AttributeId, Is.EqualTo(1));
            Assert.That(summary.Strength, Is.EqualTo(2));
            Assert.That(summary.Weakness, Is.EqualTo(1));
            Assert.That(summary.TimePeriod, Is.EqualTo("AY2023"));
            Assert.That(summary.UpdatedOn, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(summary.Attribute, Is.EqualTo(attr));
        }
    }
}
