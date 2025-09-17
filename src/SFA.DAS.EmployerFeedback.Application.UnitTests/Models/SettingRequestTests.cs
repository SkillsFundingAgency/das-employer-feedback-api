using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Models;
using System;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Models
{
    [TestFixture]
    public class SettingRequestTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var name = "TestName";
            DateTime value = DateTime.UtcNow;
            var request = new SettingRequest
            {
                Name = name,
                Value = value
            };
            Assert.That(request.Name, Is.EqualTo(name));
            Assert.That(request.Value, Is.EqualTo(value));
        }
    }
}
