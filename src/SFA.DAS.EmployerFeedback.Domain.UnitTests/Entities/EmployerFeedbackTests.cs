using FluentAssertions;
using NUnit.Framework;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class EmployerFeedbackTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var feedbackId = 1L;
            var userRef = Guid.NewGuid();
            var ukprn = 123L;
            var accountId = 456L;
            var isActive = true;
            var feedback = new Domain.Entities.EmployerFeedback
            {
                FeedbackId = feedbackId,
                UserRef = userRef,
                Ukprn = ukprn,
                AccountId = accountId,
                IsActive = isActive
            };
            feedback.FeedbackId.Should().Be(feedbackId);
            feedback.UserRef.Should().Be(userRef);
            feedback.Ukprn.Should().Be(ukprn);
            feedback.AccountId.Should().Be(accountId);
            feedback.IsActive.Should().Be(isActive);
        }
    }
}
