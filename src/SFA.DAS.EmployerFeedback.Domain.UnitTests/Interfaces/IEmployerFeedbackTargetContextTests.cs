using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Data;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Interfaces
{
    [TestFixture]
    public class EmployerFeedbackTargetContextTests
    {
        private static EmployerFeedbackDataContext CreateContext()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            var contextOptions = new DbContextOptionsBuilder<EmployerFeedbackDataContext>()
                .UseSqlite(connection)
                .Options;

            var dbContext = new EmployerFeedbackDataContext(contextOptions);
            dbContext.Database.EnsureCreated();
            
            return dbContext;
        }

        [Test]
        public async Task GetLatestResultsPerAccount_NoData_ReturnsEmptyList()
        {
            // Arrange
            var accountId = 123L;
            var userRef = Guid.NewGuid();

            await using var ctx = CreateContext();

            // Act
            var results = await ((IEmployerFeedbackTargetContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();
        }

        [Test]
        public async Task GetLatestResultsPerAccount_MultipleUkprns_IncludesTargetsWithNoResults_AndOrdersByUkprn()
        {
            // Arrange
            var accountId = 42L;
            var userRef = Guid.NewGuid();

            var ukprnWithResults = 20000000L;
            var ukprnNoResults = 15000000L;
            var dt = new DateTime(2025, 06, 01, 12, 00, 00, DateTimeKind.Utc);

            await using var ctx = CreateContext();

            var account = new Account { Id = accountId, Name = "Widgets Ltd" };
            ctx.Set<Account>().Add(account);

            var targetNoResults = new EmployerFeedbackTarget
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprnNoResults,
                Account = account,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>() // none
            };

            var targetWithResults = new EmployerFeedbackTarget
            {
                FeedbackId = 10,
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprnWithResults,
                Account = account,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 10, DateTimeCompleted = dt, ProviderRating = "Good", FeedbackSource = 1 }
                }
            };

            ctx.Set<EmployerFeedbackTarget>().AddRange(targetNoResults, targetWithResults);
            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IEmployerFeedbackTargetContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().HaveCount(2);

            // Ordered by Ukprn
            results.Select(r => r.Ukprn).Should().ContainInOrder(ukprnNoResults, ukprnWithResults);

            var noRes = results.First(r => r.Ukprn == ukprnNoResults);
            noRes.AccountName.Should().Be("Widgets Ltd");
            noRes.DateTimeCompleted.Should().BeNull();     // no results present
            noRes.ProviderRating.Should().BeNull();
            noRes.FeedbackSource.Should().BeNull();

            var withRes = results.First(r => r.Ukprn == ukprnWithResults);
            withRes.DateTimeCompleted.Should().Be(dt);
            withRes.ProviderRating.Should().Be("Good");
            withRes.FeedbackSource.Should().Be(1);
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

            var accA = new Account { Id = accountA, Name = "Account A" };
            var accB = new Account { Id = accountB, Name = "Account B" };
            ctx.Set<Account>().AddRange(accA, accB);

            // Matching pair (should be returned)
            ctx.Set<EmployerFeedbackTarget>().Add(new EmployerFeedbackTarget
            {
                FeedbackId = 1,
                AccountId = accountA,
                UserRef = userA,
                Ukprn = ukprnA,
                Account = accA,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 1, DateTimeCompleted = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc), ProviderRating = "Good", FeedbackSource = 2 }
                }
            });

            // Different account (should NOT be returned)
            ctx.Set<EmployerFeedbackTarget>().Add(new EmployerFeedbackTarget
            {
                FeedbackId = 2,
                AccountId = accountB,
                UserRef = userA,
                Ukprn = ukprnB,
                Account = accB,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 2, DateTimeCompleted = new DateTime(2025, 07, 02, 0, 0, 0, DateTimeKind.Utc), ProviderRating = "Excellent", FeedbackSource = 1 }
                }
            });

            // Different user (should NOT be returned)
            ctx.Set<EmployerFeedbackTarget>().Add(new EmployerFeedbackTarget
            {
                FeedbackId = 3,
                AccountId = accountA,
                UserRef = userB,
                Ukprn = 39999999L,
                Account = accA,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 3, DateTimeCompleted = new DateTime(2025, 07, 03, 0, 0, 0, DateTimeKind.Utc), ProviderRating = "Poor", FeedbackSource = 2 }
                }
            });

            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IEmployerFeedbackTargetContext)ctx)
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

            var account = new Account { Id = accountId, Name = "Contoso" };
            ctx.Set<Account>().Add(account);

            var target1 = new EmployerFeedbackTarget
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprn,
                Account = account,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 10, DateTimeCompleted = dtEarlier, ProviderRating = "Excellent", FeedbackSource = 1 }
                }
            };

            var target2 = new EmployerFeedbackTarget
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprn,
                Account = account,
                EmployerFeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 11, DateTimeCompleted = dtLater, ProviderRating = "Good", FeedbackSource = 2 }
                }
            };

            ctx.Set<EmployerFeedbackTarget>().AddRange(target1, target2);
            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IEmployerFeedbackTargetContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().HaveCount(1);
            var row = results.Single();
            row.Ukprn.Should().Be(ukprn);
            row.DateTimeCompleted.Should().Be(dtLater);   // picked from target2 (latest)
            row.ProviderRating.Should().Be("Good");
            row.FeedbackSource.Should().Be(2);
        }
    }
}
