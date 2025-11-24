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

        [Test]
        public async Task GetLatestResultsPerAccount_NoData_ReturnsEmptyList()
        {
            // Arrange
            var accountId = 123L;
            var userRef = Guid.NewGuid();

            await using var ctx = CreateContext();

            // Act
            var results = await ((IAccountContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();
        }

        [Test]
        public async Task GetLatestResultsPerAccount_MultipleUkprns_NotIncludesTargetsWithNoResults_AndOrdersByUkprn()
        {
            // Arrange
            var accountId = 42L;
            var userRef = Guid.NewGuid();

            var ukprnNoResults = 15000000L;
            var firstUkprnWithResults = 20000000L;
            var secondUkprnWithResults = 25000000L;
            var dt = new DateTime(2025, 06, 01, 12, 00, 00, DateTimeKind.Utc);

            await using var ctx = CreateContext();

            var account = new Account { Id = accountId, AccountName = "Widgets Ltd" };
            ctx.Set<Account>().Add(account);

            var targetNoResults = new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 9,
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprnNoResults,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>()
            };

            var firstTargetWithResults = new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 10,
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = firstUkprnWithResults,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 10,
                        DateTimeCompleted = dt,
                        ProviderRating = "Good",
                        FeedbackSource = 1
                    }
                }
            };

            var secondTargetWithResults = new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 11,
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = secondUkprnWithResults,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 11,
                        DateTimeCompleted = dt,
                        ProviderRating = "Excellent",
                        FeedbackSource = 1
                    }
                }
            };

            ctx.Set<Domain.Entities.EmployerFeedback>().AddRange(targetNoResults, firstTargetWithResults, secondTargetWithResults);
            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IAccountContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().HaveCount(2);

            results.Select(r => r.Ukprn).Should().ContainInOrder(firstUkprnWithResults, secondUkprnWithResults);
            results.Select(r => r.Ukprn).Should().NotContain(ukprnNoResults);

            var firstWithRes = results.First(r => r.Ukprn == firstUkprnWithResults);
            firstWithRes.DateTimeCompleted.Should().Be(dt);
            firstWithRes.ProviderRating.Should().Be("Good");
            firstWithRes.FeedbackSource.Should().Be(1);

            var secondWithRes = results.First(r => r.Ukprn == secondUkprnWithResults);
            secondWithRes.DateTimeCompleted.Should().Be(dt);
            secondWithRes.ProviderRating.Should().Be("Excellent");
            secondWithRes.FeedbackSource.Should().Be(1);
        }

        [Test]
        public async Task GetLatestResultsPerAccount_FiltersByAccountAndUserRef()
        {
            // Arrange
            var accountA = 100L;
            var accountB = 200L;
            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();

            var ukprnA = 30000001L;
            var ukprnB = 30000002L;

            await using var ctx = CreateContext();

            var accA = new Account { Id = accountA, AccountName = "Account A" };
            var accB = new Account { Id = accountB, AccountName = "Account B" };
            ctx.Set<Account>().AddRange(accA, accB);

            // matching account and user should be returned
            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 1,
                AccountId = accountA,
                UserRef = userA,
                Ukprn = ukprnA,
                Account = accA,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 1,
                        DateTimeCompleted = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc),
                        ProviderRating = "Good",
                        FeedbackSource = 2
                    }
                }
            });

            // different account but matching user should not be returned
            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 2,
                AccountId = accountB,
                UserRef = userA,
                Ukprn = ukprnB,
                Account = accB,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 2,
                        DateTimeCompleted = new DateTime(2025, 07, 02, 0, 0, 0, DateTimeKind.Utc),
                        ProviderRating = "Excellent",
                        FeedbackSource = 1
                    }
                }
            });

            // matching account but different user should not be returned
            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 3,
                AccountId = accountA,
                UserRef = userB,
                Ukprn = 39999999L,
                Account = accA,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 3,
                        DateTimeCompleted = new DateTime(2025, 07, 03, 0, 0, 0, DateTimeKind.Utc),
                        ProviderRating = "Poor",
                        FeedbackSource = 2
                    }
                }
            });

            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IAccountContext)ctx)
                .GetLatestResultsPerAccount(accountA, userA, CancellationToken.None);

            // Assert
            results.Should().HaveCount(1);
            var row = results.Single();

            row.AccountId.Should().Be(accountA);
            row.AccountName.Should().Be("Account A");
            row.Ukprn.Should().Be(ukprnA);
            row.ProviderRating.Should().Be("Good");
            row.FeedbackSource.Should().Be(2);
        }

        [Test]
        public async Task GetLatestResultsPerAccount_MultipleTargetsSameUkprn_SelectsGroupTopByLatestResult()
        {
            // Arrange
            var accountId = 55L;
            var userRef = Guid.NewGuid();
            var ukprn = 88888888L;

            var dtEarlier = new DateTime(2025, 01, 10, 9, 0, 0, DateTimeKind.Utc);
            var dtLater = new DateTime(2025, 02, 15, 9, 0, 0, DateTimeKind.Utc); // later -> should win

            await using var ctx = CreateContext();

            var account = new Account { Id = accountId, AccountName = "Contoso" };
            ctx.Set<Account>().Add(account);

            var earlierFeedback = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprn,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 10,
                        DateTimeCompleted = dtEarlier,
                        ProviderRating = "Excellent",
                        FeedbackSource = 1
                    }
                }
            };

            var laterFeedback = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprn,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 11,
                        DateTimeCompleted = dtLater,
                        ProviderRating = "Good",
                        FeedbackSource = 2
                    }
                }
            };

            ctx.Set<Domain.Entities.EmployerFeedback>().AddRange(earlierFeedback, laterFeedback);
            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IAccountContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().HaveCount(1);
            var row = results.Single();
            row.Ukprn.Should().Be(ukprn);
            row.DateTimeCompleted.Should().Be(dtLater);
            row.ProviderRating.Should().Be("Good");
            row.FeedbackSource.Should().Be(2);
        }

        [Test]
        public async Task GetLatestResultsPerAccount_AccountExists_NoFeedbacksForUser_ReturnsAccountRowWithNullUkprn()
        {
            // Arrange
            var accountId = 1234L;
            var user = Guid.NewGuid();
            var differentUser = Guid.NewGuid();

            await using var ctx = CreateContext();

            var account = new Account { Id = accountId, AccountName = "Acme Ltd" };
            ctx.Set<Account>().Add(account);

            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                Account = account,
                UserRef = differentUser, // feedback is for a different user
                Ukprn = 11111111L,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult
                    {
                        FeedbackId = 1,
                        DateTimeCompleted = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                        ProviderRating = "Good",
                        FeedbackSource = 1
                    }
                }
            });

            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IAccountContext)ctx)
                .GetLatestResultsPerAccount(accountId, user, CancellationToken.None);

            // Assert
            results.Should().HaveCount(1);
            var row = results.Single();
            row.AccountId.Should().Be(accountId);
            row.AccountName.Should().Be("Acme Ltd");
            row.Ukprn.Should().BeNull();
            row.DateTimeCompleted.Should().BeNull();
            row.ProviderRating.Should().BeNull();
            row.FeedbackSource.Should().BeNull();
        }
    }
}
