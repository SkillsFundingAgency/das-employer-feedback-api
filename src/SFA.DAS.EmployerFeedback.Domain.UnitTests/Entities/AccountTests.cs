using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    [TestFixture]
    public class AccountTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var checkedOn = DateTime.UtcNow;
            var account = new Account
            {
                Id = 42,
                AccountName = "TestAccount",
                CheckedOn = checkedOn
            };
            Assert.That(account.Id, Is.EqualTo(42));
            Assert.That(account.AccountName, Is.EqualTo("TestAccount"));
            Assert.That(account.CheckedOn, Is.EqualTo(checkedOn));
        }
    }
}
