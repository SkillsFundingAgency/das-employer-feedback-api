using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Data;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Interfaces
{
    [TestFixture]
    public class IFeedbackTransactionContextTests
    {
        private EmployerFeedbackDataContext _context;
        private SqliteConnection _connection;

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<EmployerFeedbackDataContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new EmployerFeedbackDataContext(options);
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
            _connection?.Dispose();
        }

        private async Task<FeedbackTransaction> GetMostRecentByAccountIdAsync(long accountId)
        {
            return await _context.FeedbackTransactions
                .AsNoTracking()
                .Where(ft => ft.AccountId == accountId)
                .OrderByDescending(ft => ft.CreatedOn)
                .FirstOrDefaultAsync();
        }

        [Test]
        public async Task GetMostRecentByAccountIdAsync_WhenNoTransactionsExist_ShouldReturnNull()
        {
            var accountId = 123L;

            var result = await GetMostRecentByAccountIdAsync(accountId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetMostRecentByAccountIdAsync_WhenSingleTransactionExists_ShouldReturnThatTransaction()
        {
            var accountId = 123L;
            var account = new Account { Id = accountId, AccountName = "Test Account" };
            var transaction = new FeedbackTransaction
            {
                Id = 1,
                AccountId = accountId,
                TemplateName = "Test Template",
                SendAfter = DateTime.UtcNow.AddDays(30),
                CreatedOn = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            _context.FeedbackTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            var result = await GetMostRecentByAccountIdAsync(accountId);

            result.Should().NotBeNull();
            result.Id.Should().Be(transaction.Id);
            result.AccountId.Should().Be(accountId);
            result.TemplateName.Should().Be(transaction.TemplateName);
        }

        [Test]
        public async Task GetMostRecentByAccountIdAsync_WhenMultipleTransactionsExist_ShouldReturnMostRecent()
        {
            var accountId = 123L;
            var account = new Account { Id = accountId, AccountName = "Test Account" };
            var now = DateTime.UtcNow;
            var oldTransaction = new FeedbackTransaction
            {
                Id = 1,
                AccountId = accountId,
                TemplateName = "Old Template",
                SendAfter = now.AddDays(30),
                CreatedOn = now.AddDays(-10)
            };
            var recentTransaction = new FeedbackTransaction
            {
                Id = 2,
                AccountId = accountId,
                TemplateName = "Recent Template",
                SendAfter = now.AddDays(30),
                CreatedOn = now.AddDays(-1)
            };

            _context.Accounts.Add(account);
            _context.FeedbackTransactions.AddRange(oldTransaction, recentTransaction);
            await _context.SaveChangesAsync();

            var result = await GetMostRecentByAccountIdAsync(accountId);

            result.Should().NotBeNull();
            result.Id.Should().Be(recentTransaction.Id);
            result.TemplateName.Should().Be("Recent Template");
            result.CreatedOn.Should().BeCloseTo(recentTransaction.CreatedOn, TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task GetMostRecentByAccountIdAsync_WhenTransactionsForDifferentAccounts_ShouldReturnCorrectOne()
        {
            var accountId1 = 123L;
            var accountId2 = 456L;
            var now = DateTime.UtcNow;
            var account1 = new Account { Id = accountId1, AccountName = "Test Account 1" };
            var account2 = new Account { Id = accountId2, AccountName = "Test Account 2" };
            var transaction1 = new FeedbackTransaction
            {
                Id = 1,
                AccountId = accountId1,
                TemplateName = "Template 1",
                SendAfter = now.AddDays(30),
                CreatedOn = now.AddDays(-5)
            };
            var transaction2 = new FeedbackTransaction
            {
                Id = 2,
                AccountId = accountId2,
                TemplateName = "Template 2",
                SendAfter = now.AddDays(30),
                CreatedOn = now.AddDays(-1)
            };

            _context.Accounts.AddRange(account1, account2);
            _context.FeedbackTransactions.AddRange(transaction1, transaction2);
            await _context.SaveChangesAsync();

            var result = await GetMostRecentByAccountIdAsync(accountId1);

            result.Should().NotBeNull();
            result.Id.Should().Be(transaction1.Id);
            result.AccountId.Should().Be(accountId1);
            result.TemplateName.Should().Be("Template 1");
        }

        [Test]
        public async Task AddAsync_ShouldAddFeedbackTransaction()
        {
            var accountId = 123L;
            var account = new Account { Id = accountId, AccountName = "Test Account" };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            var transaction = new FeedbackTransaction
            {
                AccountId = accountId,
                TemplateName = "Test Template",
                SendAfter = now.AddDays(30),
                CreatedOn = now
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            var savedTransaction = await GetMostRecentByAccountIdAsync(accountId);
            savedTransaction.Should().NotBeNull();
            savedTransaction.TemplateName.Should().Be("Test Template");
            savedTransaction.AccountId.Should().Be(accountId);
            savedTransaction.CreatedOn.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        }
    }
}