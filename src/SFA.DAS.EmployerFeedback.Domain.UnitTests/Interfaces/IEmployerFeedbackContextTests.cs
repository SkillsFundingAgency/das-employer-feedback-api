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
    public class EmployerFeedbackContextTests
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
            var results = await ((IEmployerFeedbackContext)ctx)
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
                FeedbackResults = new List<EmployerFeedbackResult>() // none
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
                    new EmployerFeedbackResult { FeedbackId = 10, DateTimeCompleted = dt, ProviderRating = "Good", FeedbackSource = 1 }
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
                    new EmployerFeedbackResult { FeedbackId = 11, DateTimeCompleted = dt, ProviderRating = "Excellent", FeedbackSource = 1 }
                }
            };

            ctx.Set<Domain.Entities.EmployerFeedback>().AddRange(targetNoResults, firstTargetWithResults, secondTargetWithResults);
            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IEmployerFeedbackContext)ctx)
                .GetLatestResultsPerAccount(accountId, userRef, CancellationToken.None);

            // Assert
            results.Should().HaveCount(2);

            // Ordered by Ukprn
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

            // Matching pair (should be returned)
            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 1,
                AccountId = accountA,
                UserRef = userA,
                Ukprn = ukprnA,
                Account = accA,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 1, DateTimeCompleted = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc), ProviderRating = "Good", FeedbackSource = 2 }
                }
            });

            // Different account (should NOT be returned)
            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 2,
                AccountId = accountB,
                UserRef = userA,
                Ukprn = ukprnB,
                Account = accB,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 2, DateTimeCompleted = new DateTime(2025, 07, 02, 0, 0, 0, DateTimeKind.Utc), ProviderRating = "Excellent", FeedbackSource = 1 }
                }
            });

            // Different user (should NOT be returned)
            ctx.Set<Domain.Entities.EmployerFeedback>().Add(new Domain.Entities.EmployerFeedback
            {
                FeedbackId = 3,
                AccountId = accountA,
                UserRef = userB,
                Ukprn = 39999999L,
                Account = accA,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 3, DateTimeCompleted = new DateTime(2025, 07, 03, 0, 0, 0, DateTimeKind.Utc), ProviderRating = "Poor", FeedbackSource = 2 }
                }
            });

            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IEmployerFeedbackContext)ctx)
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

            var target1 = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprn,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 10, DateTimeCompleted = dtEarlier, ProviderRating = "Excellent", FeedbackSource = 1 }
                }
            };

            var target2 = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                UserRef = userRef,
                Ukprn = ukprn,
                Account = account,
                FeedbackResults = new List<EmployerFeedbackResult>
                {
                    new EmployerFeedbackResult { FeedbackId = 11, DateTimeCompleted = dtLater, ProviderRating = "Good", FeedbackSource = 2 }
                }
            };

            ctx.Set<Domain.Entities.EmployerFeedback>().AddRange(target1, target2);
            await ctx.SaveChangesAsync();

            // Act
            var results = await ((IEmployerFeedbackContext)ctx)
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
