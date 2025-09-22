using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Models
{
    [TestFixture]
    public class UpsertAccountsRequestTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "Test" } };
            var request = new UpsertAccountsRequest
            {
                Accounts = accounts
            };
            Assert.That(request.Accounts, Is.EqualTo(accounts));
        }
    }
}
