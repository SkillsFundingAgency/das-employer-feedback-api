using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class ProviderAttributeTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var feedbackResultId = Guid.NewGuid();
            var attributeId = 123L;
            var attributeValue = 1;
            var feedbackResult = new EmployerFeedbackResult { Id = feedbackResultId };
            var attribute = new Attributes { AttributeId = attributeId, AttributeName = "Test Attribute" };
            var providerAttribute = new ProviderAttribute
            {
                EmployerFeedbackResultId = feedbackResultId,
                AttributeId = attributeId,
                AttributeValue = attributeValue,
                EmployerFeedbackResult = feedbackResult,
                Attribute = attribute
            };
            providerAttribute.EmployerFeedbackResultId.Should().Be(feedbackResultId);
            providerAttribute.AttributeId.Should().Be(attributeId);
            providerAttribute.AttributeValue.Should().Be(attributeValue);
            providerAttribute.EmployerFeedbackResult.Should().Be(feedbackResult);
            providerAttribute.Attribute.Should().Be(attribute);
        }
    }
}
