using NUnit.Framework;
using FluentAssertions;
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
            
            account.Id.Should().Be(42);
            account.AccountName.Should().Be("TestAccount");
            account.CheckedOn.Should().Be(checkedOn);
        }
    }
}
