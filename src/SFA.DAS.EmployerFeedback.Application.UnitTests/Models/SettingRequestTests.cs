using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Models;
using System;
using System.Globalization;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Models
{
    [TestFixture]
    public class SettingRequestTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var request = new SettingRequest
            {
                Value = value
            };
            Assert.That(request.Value, Is.EqualTo(value));
        }
    }
}
