using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction;
using System;
using AutoFixture.NUnit3;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetFeedbackTransaction
{
    [TestFixture]
    public class GetFeedbackTransactionQueryResultTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(
            long id,
            long accountId,
            string accountName,
            string templateName,
            Guid templateId,
            DateTime createdOn,
            DateTime sendAfter,
            int sentCount,
            DateTime sentDate)
        {
            var result = new GetFeedbackTransactionQueryResult
            {
                Id = id,
                AccountId = accountId,
                AccountName = accountName,
                TemplateName = templateName,
                TemplateId = templateId,
                CreatedOn = createdOn,
                SendAfter = sendAfter,
                SentCount = sentCount,
                SentDate = sentDate
            };

            result.Id.Should().Be(id);
            result.AccountId.Should().Be(accountId);
            result.AccountName.Should().Be(accountName);
            result.TemplateName.Should().Be(templateName);
            result.TemplateId.Should().Be(templateId);
            result.CreatedOn.Should().Be(createdOn);
            result.SendAfter.Should().Be(sendAfter);
            result.SentCount.Should().Be(sentCount);
            result.SentDate.Should().Be(sentDate);
        }
    }
}