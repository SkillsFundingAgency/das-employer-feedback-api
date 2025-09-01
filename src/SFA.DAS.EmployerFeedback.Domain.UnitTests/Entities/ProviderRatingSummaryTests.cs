using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class ProviderRatingSummaryTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(long ukprn, string rating, int ratingCount, string timePeriod, DateTime updatedOn)
        {
            var attributes = new ProviderRatingSummary
            {
                Ukprn = ukprn,
                Rating = rating,
                RatingCount = ratingCount,
                TimePeriod = timePeriod,
                UpdatedOn = updatedOn,
            };

            attributes.Ukprn.Should().Be(ukprn);
            attributes.Rating.Should().Be(rating);
            attributes.RatingCount.Should().Be(ratingCount);
            attributes.TimePeriod.Should().Be(timePeriod);
            attributes.UpdatedOn.Should().Be(updatedOn);
        }
    }
}
