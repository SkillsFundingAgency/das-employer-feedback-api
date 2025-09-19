using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Models
{
    public class AllEmployerFeedbackResultsTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var attributes = new List<ProviderAttributeResults>
            {
                new ProviderAttributeResults { Name = "Quality", Value = 1 }
            };
            var model = new AllEmployerFeedbackResults
            {
                Ukprn = 12345678,
                DateTimeCompleted = new DateTime(2024, 1, 1),
                ProviderRating = "Excellent",
                ProviderAttributes = attributes
            };
            Assert.That(model.Ukprn, Is.EqualTo(12345678));
            Assert.That(model.DateTimeCompleted, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(model.ProviderRating, Is.EqualTo("Excellent"));
            Assert.That(model.ProviderAttributes, Is.EqualTo(attributes));
        }
    }
}
