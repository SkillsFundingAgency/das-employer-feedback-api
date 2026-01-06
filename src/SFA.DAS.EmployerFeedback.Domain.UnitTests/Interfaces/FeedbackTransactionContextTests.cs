using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Data;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Interfaces
{
    [TestFixture]
    public class FeedbackTransactionContextTests
    {
        private static EmployerFeedbackDataContext CreateContext()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var contextOptions = new DbContextOptionsBuilder<EmployerFeedbackDataContext>()
                .UseSqlite(connection)
                .Options;

            var dbContext = new EmployerFeedbackDataContext(contextOptions);
            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        [Test]
        public async Task GetFeedbackTransactionsBatchAsync_ReturnsCorrectTransactions_WhenSendAfterIsInPastAndSentDateIsNull()
        {
            await using var context = CreateContext();
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var futureDate = DateTime.UtcNow.AddDays(1);
            var currentDateTime = DateTime.UtcNow;

            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Account 1" },
                new Account { Id = 2, AccountName = "Account 2" },
                new Account { Id = 3, AccountName = "Account 3" },
                new Account { Id = 4, AccountName = "Account 4" },
                new Account { Id = 5, AccountName = "Account 5" }
            };
            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var feedbackTransactions = new List<FeedbackTransaction>
            {
                new FeedbackTransaction { AccountId = 1, TemplateName = "Test", SendAfter = pastDate, SentDate = null, CreatedOn = DateTime.UtcNow },
                new FeedbackTransaction { AccountId = 2, TemplateName = "Test", SendAfter = pastDate, SentDate = DateTime.UtcNow, CreatedOn = DateTime.UtcNow },
                new FeedbackTransaction { AccountId = 3, TemplateName = "Test", SendAfter = futureDate, SentDate = null, CreatedOn = DateTime.UtcNow },
                new FeedbackTransaction { AccountId = 5, TemplateName = "Test", SendAfter = pastDate, SentDate = null, CreatedOn = DateTime.UtcNow },
                new FeedbackTransaction { AccountId = 4, TemplateName = "Test", SendAfter = pastDate, SentDate = null, CreatedOn = DateTime.UtcNow },
            };

            context.FeedbackTransactions.AddRange(feedbackTransactions);
            await context.SaveChangesAsync();

            var result = await ((IFeedbackTransactionContext)context).GetFeedbackTransactionsBatchAsync(3, currentDateTime);

            result.Should().HaveCount(3);
            result.Should().BeInAscendingOrder();
        }

        [Test]
        public async Task GetFeedbackTransactionsBatchAsync_ReturnsEmptyList_WhenNoValidTransactions()
        {
            await using var context = CreateContext();
            var futureDate = DateTime.UtcNow.AddDays(1);
            var currentDateTime = DateTime.UtcNow;

            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Account 1" },
                new Account { Id = 2, AccountName = "Account 2" }
            };
            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var feedbackTransactions = new List<FeedbackTransaction>
            {
                new FeedbackTransaction { AccountId = 1, TemplateName = "Test", SendAfter = futureDate, SentDate = null, CreatedOn = DateTime.UtcNow },
                new FeedbackTransaction { AccountId = 2, TemplateName = "Test", SendAfter = DateTime.UtcNow.AddDays(-1), SentDate = DateTime.UtcNow, CreatedOn = DateTime.UtcNow },
            };

            context.FeedbackTransactions.AddRange(feedbackTransactions);
            await context.SaveChangesAsync();

            var result = await ((IFeedbackTransactionContext)context).GetFeedbackTransactionsBatchAsync(5, currentDateTime);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetFeedbackTransactionsBatchAsync_RespectsBatchSize()
        {
            await using var context = CreateContext();
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var currentDateTime = DateTime.UtcNow;

            var accounts = new List<Account>();
            for (int i = 1; i <= 10; i++)
            {
                accounts.Add(new Account { Id = i, AccountName = $"Account {i}" });
            }
            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var feedbackTransactions = new List<FeedbackTransaction>();
            for (int i = 1; i <= 10; i++)
            {
                feedbackTransactions.Add(new FeedbackTransaction { AccountId = i, TemplateName = "Test", SendAfter = pastDate, SentDate = null, CreatedOn = DateTime.UtcNow });
            }

            context.FeedbackTransactions.AddRange(feedbackTransactions);
            await context.SaveChangesAsync();

            var result = await ((IFeedbackTransactionContext)context).GetFeedbackTransactionsBatchAsync(3, currentDateTime);

            result.Should().HaveCount(3);
            result.Should().BeInAscendingOrder();
        }

        private static async Task<FeedbackTransactionSummary> GetMostRecentSummaryByAccountIdAsync(EmployerFeedbackDataContext context, long accountId)
        {
            return await ((IFeedbackTransactionContext)context).GetMostRecentSummaryByAccountIdAsync(accountId);
        }

        [Test]
        public async Task GetMostRecentSummaryByAccountIdAsync_WhenNoTransactionsExist_ShouldReturnNull()
        {
            await using var context = CreateContext();
            var accountId = 999L;

            var result = await GetMostRecentSummaryByAccountIdAsync(context, accountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetMostRecentSummaryByAccountIdAsync_WhenTransactionsExist_ShouldReturnSummary()
        {
            await using var context = CreateContext();
            var accountId = 222L;
            var account = new Account { Id = accountId, AccountName = "Summary Account" };
            var now = DateTime.UtcNow;
            var transaction = new FeedbackTransaction
            {
                Id = 10,
                AccountId = accountId,
                TemplateName = "Summary Template",
                SendAfter = now.AddDays(5),
                SentDate = null,
                CreatedOn = now
            };

            context.Accounts.Add(account);
            context.FeedbackTransactions.Add(transaction);
            await context.SaveChangesAsync();

            var result = await GetMostRecentSummaryByAccountIdAsync(context, accountId);

            result.Should().NotBeNull();
            result.Id.Should().Be(transaction.Id);
            result.AccountId.Should().Be(accountId);
            result.SendAfter.Should().Be(transaction.SendAfter);
            result.SentDate.Should().BeNull();
        }

        [Test]
        public async Task GetMostRecentSummaryByAccountIdAsync_WhenMultipleTransactionsExist_ShouldReturnMostRecentById()
        {
            await using var context = CreateContext();
            var accountId = 333L;
            var account = new Account { Id = accountId, AccountName = "Summary Account Multiple" };

            var t1 = new FeedbackTransaction { Id = 5, AccountId = accountId, SendAfter = DateTime.UtcNow.AddDays(10), SentDate = null, CreatedOn = DateTime.UtcNow.AddDays(-3) };
            var t2 = new FeedbackTransaction { Id = 9, AccountId = accountId, SendAfter = DateTime.UtcNow.AddDays(20), SentDate = DateTime.UtcNow.AddDays(-1), CreatedOn = DateTime.UtcNow.AddDays(-1) };

            context.Accounts.Add(account);
            context.FeedbackTransactions.AddRange(t1, t2);
            await context.SaveChangesAsync();

            var result = await GetMostRecentSummaryByAccountIdAsync(context, accountId);

            result.Should().NotBeNull();
            result.Id.Should().Be(t2.Id);
            result.SendAfter.Should().Be(t2.SendAfter);
            result.SentDate.Should().Be(t2.SentDate);
        }
    }
}