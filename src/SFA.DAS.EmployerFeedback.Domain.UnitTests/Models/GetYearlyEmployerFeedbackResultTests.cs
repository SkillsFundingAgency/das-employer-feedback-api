using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Models
{
    public class GetYearlyEmployerFeedbackResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var attributes = new List<GetYearlyEmployerFeedbackResultProviderAttribute>
            {
                new GetYearlyEmployerFeedbackResultProviderAttribute { Name = "test", Strength = 2, Weakness = 1 }
            };
            var model = new GetYearlyEmployerFeedbackResult
            {
                Ukprn = 12345678,
                Stars = 5,
                ReviewCount = 20,
                TimePeriod = "2023",
                ProviderAttribute = attributes
            };
            Assert.That(model.Ukprn, Is.EqualTo(12345678));
            Assert.That(model.Stars, Is.EqualTo(5));
            Assert.That(model.ReviewCount, Is.EqualTo(20));
            Assert.That(model.TimePeriod, Is.EqualTo("2023"));
            Assert.That(model.ProviderAttribute, Is.EqualTo(attributes));
        }

        [Test]
        public void ProviderAttributeProperties_SetAndGet_ShouldReturnExpectedValues()
        {
            var attr = new GetYearlyEmployerFeedbackResultProviderAttribute
            {
                Name = "test",
                Strength = 3,
                Weakness = 0
            };
            Assert.That(attr.Name, Is.EqualTo("test"));
            Assert.That(attr.Strength, Is.EqualTo(3));
            Assert.That(attr.Weakness, Is.EqualTo(0));
        }

        [Test]
        public void Properties_NullOrEmptyAttributes_ShouldBeHandled()
        {
            var model = new GetYearlyEmployerFeedbackResult
            {
                Ukprn = 1,
                Stars = 0,
                ReviewCount = 0,
                TimePeriod = null,
                ProviderAttribute = null
            };
            Assert.That(model.ProviderAttribute, Is.Null);
            model.ProviderAttribute = new List<GetYearlyEmployerFeedbackResultProviderAttribute>();
            Assert.That(model.ProviderAttribute, Is.Empty);
        }
    }
}
