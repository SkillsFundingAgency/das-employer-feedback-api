using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    [TestFixture]
    public class FeedbackTransactionTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var id = 123L;
            var accountId = 456L;
            var templateName = "Test Template";
            var sendAfter = DateTime.UtcNow.AddDays(30);
            var templateId = Guid.NewGuid();
            var sentCount = 5;
            var sentDate = DateTime.UtcNow.AddDays(-1);
            var createdOn = DateTime.UtcNow.AddDays(-10);

            var feedbackTransaction = new FeedbackTransaction
            {
                Id = id,
                AccountId = accountId,
                TemplateName = templateName,
                SendAfter = sendAfter,
                TemplateId = templateId,
                SentCount = sentCount,
                SentDate = sentDate,
                CreatedOn = createdOn
            };

            feedbackTransaction.Id.Should().Be(id);
            feedbackTransaction.AccountId.Should().Be(accountId);
            feedbackTransaction.TemplateName.Should().Be(templateName);
            feedbackTransaction.SendAfter.Should().Be(sendAfter);
            feedbackTransaction.TemplateId.Should().Be(templateId);
            feedbackTransaction.SentCount.Should().Be(sentCount);
            feedbackTransaction.SentDate.Should().Be(sentDate);
            feedbackTransaction.CreatedOn.Should().Be(createdOn);
        }

        [Test]
        public void NullableProperties_SetToNull_ShouldReturnNull()
        {
            var feedbackTransaction = new FeedbackTransaction
            {
                TemplateId = null,
                SentCount = null,
                SentDate = null
            };

            feedbackTransaction.TemplateId.Should().BeNull();
            feedbackTransaction.SentCount.Should().BeNull();
            feedbackTransaction.SentDate.Should().BeNull();
        }

        [Test]
        public void Account_Navigation_Property_ShouldWork()
        {
            var account = new Account { Id = 123, AccountName = "Test Account" };
            var feedbackTransaction = new FeedbackTransaction
            {
                AccountId = account.Id,
                Account = account
            };

            feedbackTransaction.Account.Should().Be(account);
            feedbackTransaction.Account.Id.Should().Be(account.Id);
            feedbackTransaction.Account.AccountName.Should().Be(account.AccountName);
        }
    }
}