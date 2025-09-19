using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertAccounts
{
    [TestFixture]
    public class AccountUpsertDtoTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var dto = new AccountUpsertDto
            {
                AccountId = 123,
                AccountName = "TestAccount"
            };
            Assert.That(dto.AccountId, Is.EqualTo(123));
            Assert.That(dto.AccountName, Is.EqualTo("TestAccount"));
        }
    }
}
