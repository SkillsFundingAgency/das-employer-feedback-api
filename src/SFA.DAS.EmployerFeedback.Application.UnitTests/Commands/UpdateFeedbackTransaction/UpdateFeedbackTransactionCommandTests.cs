using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using System;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpdateFeedbackTransaction
{
    [TestFixture]
    public class UpdateFeedbackTransactionCommandTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var id = 123L;
            var templateId = Guid.NewGuid();
            var sentCount = 5;
            var sentDate = DateTime.UtcNow;

            var command = new UpdateFeedbackTransactionCommand
            {
                Id = id,
                TemplateId = templateId,
                SentCount = sentCount,
                SentDate = sentDate
            };

            command.Id.Should().Be(id);
            command.TemplateId.Should().Be(templateId);
            command.SentCount.Should().Be(sentCount);
            command.SentDate.Should().Be(sentDate);
        }

        [Test]
        public void ImplicitOperator_FromRequest_ShouldMapProperties()
        {
            var templateId = Guid.NewGuid();
            var sentCount = 3;
            var sentDate = DateTime.UtcNow;

            var request = new UpdateFeedbackTransactionRequest
            {
                TemplateId = templateId,
                SentCount = sentCount,
                SentDate = sentDate
            };

            UpdateFeedbackTransactionCommand command = request;

            command.TemplateId.Should().Be(templateId);
            command.SentCount.Should().Be(sentCount);
            command.SentDate.Should().Be(sentDate);
            command.Id.Should().Be(0);
        }
    }
}