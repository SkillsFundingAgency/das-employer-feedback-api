using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using SFA.DAS.EmployerFeedback.Application.Models;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Models
{
    [TestFixture]
    public class SubmitEmployerFeedbackRequestTests
    {
        [Test]
        public void Can_Create_Request_With_Valid_Properties()
        {
            var userRef = Guid.NewGuid();
            var ukprn = 12345L;
            var accountId = 67890L;
            var providerRating = OverallRating.Good;
            var feedbackSource = 1;
            var providerAttributes = new List<ProviderAttributeDto>
            {
                new ProviderAttributeDto { AttributeId = 1, AttributeValue = 1 },
                new ProviderAttributeDto { AttributeId = 2, AttributeValue = -1 }
            };

            var request = new SubmitEmployerFeedbackRequest
            {
                UserRef = userRef,
                Ukprn = ukprn,
                AccountId = accountId,
                ProviderRating = providerRating,
                FeedbackSource = feedbackSource,
                ProviderAttributes = providerAttributes
            };

            Assert.That(request.UserRef, Is.EqualTo(userRef));
            Assert.That(request.Ukprn, Is.EqualTo(ukprn));
            Assert.That(request.AccountId, Is.EqualTo(accountId));
            Assert.That(request.ProviderRating, Is.EqualTo(providerRating));
            Assert.That(request.FeedbackSource, Is.EqualTo(feedbackSource));
            Assert.That(request.ProviderAttributes, Is.EqualTo(providerAttributes));
        }

        [Test]
        public void ProviderAttributes_Can_Be_Null()
        {
            var request = new SubmitEmployerFeedbackRequest
            {
                ProviderAttributes = null
            };
            Assert.That(request.ProviderAttributes, Is.Null);
        }

        [Test]
        public void ProviderAttributes_Can_Be_Empty()
        {
            var request = new SubmitEmployerFeedbackRequest
            {
                ProviderAttributes = new List<ProviderAttributeDto>()
            };
            Assert.That(request.ProviderAttributes, Is.Empty);
        }
    }
}
