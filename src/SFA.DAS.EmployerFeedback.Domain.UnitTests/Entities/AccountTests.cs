using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class AccountTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var account = new Account
            {
                Id = 123,
                Name = "test"
            };
            Assert.That(account.Id, Is.EqualTo(123));
            Assert.That(account.Name, Is.EqualTo("test"));
        }
    }
}
