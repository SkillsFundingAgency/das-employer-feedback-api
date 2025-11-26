using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Models;
using System;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Models
{
    [TestFixture]
    public class UpdateFeedbackTransactionRequestTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
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

            Assert.That(request.TemplateId, Is.EqualTo(templateId));
            Assert.That(request.SentCount, Is.EqualTo(sentCount));
            Assert.That(request.SentDate, Is.EqualTo(sentDate));
        }
    }
}