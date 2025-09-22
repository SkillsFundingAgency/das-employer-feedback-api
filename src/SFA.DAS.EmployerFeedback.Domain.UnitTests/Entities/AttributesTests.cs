using SFA.DAS.EmployerFeedback.Domain.Entities;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class AttributesTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(long id, string name)
        {
            var attributes = new Attributes
            {
                AttributeId = id,
                AttributeName = name
            };

            attributes.AttributeId.Should().Be(id);
            attributes.AttributeName.Should().Be(name);
        }
    }
}
