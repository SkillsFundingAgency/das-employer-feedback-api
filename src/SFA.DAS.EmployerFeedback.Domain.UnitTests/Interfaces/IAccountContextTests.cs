using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Data;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Interfaces
{
    [TestFixture]
    public class IAccountContextTests
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
        public async Task GetEmailNudgeAccountsBatchAsync_ReturnsAccountsWithNullCheckedOnFirst()
        {
            await using var context = CreateContext();

            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Account 1", CheckedOn = null },
                new Account { Id = 2, AccountName = "Account 2", CheckedOn = DateTime.UtcNow.AddDays(-40) },
                new Account { Id = 3, AccountName = "Account 3", CheckedOn = null },
                new Account { Id = 4, AccountName = "Account 4", CheckedOn = DateTime.UtcNow.AddDays(-35) }
            };

            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var result = await ((IAccountContext)context).GetEmailNudgeAccountsBatchAsync(10, 30, CancellationToken.None);

            result.Should().HaveCount(4);
            result.Take(2).Should().BeEquivalentTo(new[] { 1, 3 });
            result.Skip(2).Should().BeEquivalentTo(new[] { 2, 4 });
        }

        [Test]
        public async Task GetEmailNudgeAccountsBatchAsync_FiltersAccountsWithinEmailNudgeCheckDays()
        {
            await using var context = CreateContext();

            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Account 1", CheckedOn = null },
                new Account { Id = 2, AccountName = "Account 2", CheckedOn = DateTime.UtcNow.AddDays(-40) },
                new Account { Id = 3, AccountName = "Account 3", CheckedOn = DateTime.UtcNow.AddDays(-10) },
                new Account { Id = 4, AccountName = "Account 4", CheckedOn = DateTime.UtcNow.AddDays(-35) }
            };

            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var result = await ((IAccountContext)context).GetEmailNudgeAccountsBatchAsync(10, 30, CancellationToken.None);

            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(new[] { 1, 2, 4 });
        }

        [Test]
        public async Task GetEmailNudgeAccountsBatchAsync_RespectsPageSize()
        {
            await using var context = CreateContext();

            var accounts = new List<Account>();
            for (int i = 1; i <= 10; i++)
            {
                accounts.Add(new Account { Id = i, AccountName = $"Account {i}", CheckedOn = null });
            }

            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var result = await ((IAccountContext)context).GetEmailNudgeAccountsBatchAsync(5, 30, CancellationToken.None);

            result.Should().HaveCount(5);
            result.Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5 });
        }

        [Test]
        public async Task GetEmailNudgeAccountsBatchAsync_ReturnsEmptyWhenNoAccountsMatch()
        {
            await using var context = CreateContext();

            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Account 1", CheckedOn = DateTime.UtcNow.AddDays(-5) },
                new Account { Id = 2, AccountName = "Account 2", CheckedOn = DateTime.UtcNow.AddDays(-10) }
            };

            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var result = await ((IAccountContext)context).GetEmailNudgeAccountsBatchAsync(10, 30, CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetEmailNudgeAccountsBatchAsync_OrdersByCheckOnThenById()
        {
            await using var context = CreateContext();

            var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var accounts = new List<Account>
            {
                new Account { Id = 5, AccountName = "Account 5", CheckedOn = baseDate.AddDays(-40) },
                new Account { Id = 3, AccountName = "Account 3", CheckedOn = null },
                new Account { Id = 1, AccountName = "Account 1", CheckedOn = null },
                new Account { Id = 4, AccountName = "Account 4", CheckedOn = baseDate.AddDays(-50) },
                new Account { Id = 2, AccountName = "Account 2", CheckedOn = baseDate.AddDays(-45) }
            };

            context.Accounts.AddRange(accounts);
            await context.SaveChangesAsync();

            var result = await ((IAccountContext)context).GetEmailNudgeAccountsBatchAsync(10, 30, CancellationToken.None);

            result.Should().HaveCount(5);
            result.Should().BeEquivalentTo(new[] { 1, 3, 4, 2, 5 }, options => options.WithStrictOrdering());
        }
    }
}
