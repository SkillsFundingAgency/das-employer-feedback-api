using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;
using SFA.DAS.EmployerFeedback.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertAccounts
{
    [TestFixture]
    public class UpsertAccountsCommandTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "Test" } };
            var command = new UpsertAccountsCommand
            {
                Accounts = accounts
            };
            Assert.That(command.Accounts, Is.EqualTo(accounts));
        }

        [Test]
        public void ImplicitConversion_FromRequest_ShouldMapProperties()
        {
            var accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 2, AccountName = "Test2" } };
            var request = new UpsertAccountsRequest { Accounts = accounts };
            UpsertAccountsCommand command = request;
            Assert.That(command.Accounts, Is.EqualTo(accounts));
        }
    }
}
