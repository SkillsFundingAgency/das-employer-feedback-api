using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertAccounts
{
    [TestFixture]
    public class UpsertAccountsCommandHandlerTests
    {
        private Mock<IAccountContext> _accountContext;
        private Mock<ILogger<UpsertAccountsCommandHandler>> _logger;
        private UpsertAccountsCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _accountContext = new Mock<IAccountContext>();
            _logger = new Mock<ILogger<UpsertAccountsCommandHandler>>();
            _handler = new UpsertAccountsCommandHandler(_accountContext.Object, _logger.Object);
        }

        [Test]
        public async Task Handle_Should_Add_New_Account_If_Not_Exists()
        {
            var command = new UpsertAccountsCommand
            {
                Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "Test" } }
            };
            _accountContext.Setup(x => x.GetAccountsByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Account>());
            _accountContext.Setup(x => x.Add(It.IsAny<Account>())).Verifiable();
            _accountContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);
            _accountContext.Verify(x => x.Add(It.IsAny<Account>()), Times.Once);
            _accountContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Update_Account_If_Exists()
        {
            var existing = new Account { Id = 2, AccountName = "OldName" };
            var command = new UpsertAccountsCommand
            {
                Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 2, AccountName = "NewName" } }
            };
            _accountContext.Setup(x => x.GetAccountsByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Account> { existing });
            _accountContext.Setup(x => x.Update(existing)).Verifiable();
            _accountContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);
            _accountContext.Verify(x => x.Update(existing), Times.Once);
            _accountContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_Should_Log_And_Throw_On_Exception()
        {
            var command = new UpsertAccountsCommand
            {
                Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "Test" } }
            };
            _accountContext.Setup(x => x.GetAccountsByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Account>());
            _accountContext.Setup(x => x.Add(It.IsAny<Account>())).Verifiable();
            _accountContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));

            Assert.ThrowsAsync<System.Exception>(async () => await _handler.Handle(command, CancellationToken.None));
            _logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error upserting")),
                It.IsAny<System.Exception>(),
                It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()), Times.Once);
        }
    }
}
