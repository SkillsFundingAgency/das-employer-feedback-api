using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class EmployerFeedbackResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var id = Guid.NewGuid();
            var feedbackId = 123L;
            var dateTimeCompleted = DateTime.UtcNow;
            var providerRating = "Excellent";
            var feedbackSource = 1;
            var result = new EmployerFeedbackResult
            {
                Id = id,
                FeedbackId = feedbackId,
                DateTimeCompleted = dateTimeCompleted,
                ProviderRating = providerRating,
                FeedbackSource = feedbackSource
            };
            result.Id.Should().Be(id);
            result.FeedbackId.Should().Be(feedbackId);
            result.DateTimeCompleted.Should().Be(dateTimeCompleted);
            result.ProviderRating.Should().Be(providerRating);
            result.FeedbackSource.Should().Be(feedbackSource);
        }
    }
}
