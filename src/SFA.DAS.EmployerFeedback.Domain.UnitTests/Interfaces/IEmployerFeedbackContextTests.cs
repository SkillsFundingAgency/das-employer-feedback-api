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
        public async Task GetByUserUkprnAccountAsync_ReturnsMatchingEntity_OrNullWhenNotFound()
        {
            // Arrange
            var accountId = 321L;
            var userRef = Guid.NewGuid();
            var ukprn = 12345678L;

            await using var ctx = CreateContext();

            var account = new Account { Id = accountId, AccountName = "Acme Ltd" };
            ctx.Set<Account>().Add(account);

            var matching = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                Account = account,
                UserRef = userRef,
                Ukprn = ukprn,
                FeedbackResults = new List<EmployerFeedbackResult>()
            };

            var differentUser = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                Account = account,
                UserRef = Guid.NewGuid(), // not matching
                Ukprn = ukprn,
                FeedbackResults = new List<EmployerFeedbackResult>()
            };

            var differentAccount = new Domain.Entities.EmployerFeedback
            {
                AccountId = 999L,
                Account = new Account { Id = 999L, AccountName = "Other Ltd" },
                UserRef = userRef,
                Ukprn = ukprn,
                FeedbackResults = new List<EmployerFeedbackResult>()
            };

            var differentUkprn = new Domain.Entities.EmployerFeedback
            {
                AccountId = accountId,
                Account = account,
                UserRef = userRef,
                Ukprn = 98765432L, // not matching
                FeedbackResults = new List<EmployerFeedbackResult>()
            };

            ctx.Set<Domain.Entities.EmployerFeedback>().AddRange(matching, differentUser, differentAccount, differentUkprn);
            await ctx.SaveChangesAsync();

            // Act
            var result = await ((IEmployerFeedbackContext)ctx)
                .GetByUserUkprnAccountAsync(userRef, ukprn, accountId, CancellationToken.None);

            var notFound = await ((IEmployerFeedbackContext)ctx)
                .GetByUserUkprnAccountAsync(Guid.NewGuid(), 99999999L, 888L, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.AccountId.Should().Be(accountId);
            result.UserRef.Should().Be(userRef);
            result.Ukprn.Should().Be(ukprn);

            notFound.Should().BeNull();
        }
    }
}
