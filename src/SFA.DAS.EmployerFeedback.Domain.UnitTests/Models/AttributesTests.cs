using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;
using ModelAttributes = SFA.DAS.EmployerFeedback.Domain.Models.Attributes;
using EntityAttributes = SFA.DAS.EmployerFeedback.Domain.Entities.Attributes;
                            
namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Models
{
    public class AttributesTests
    {
        [Test]
        public void ImplicitOperator_SourceIsNull_ReturnsNull()
        {
            EntityAttributes source = null;
            ModelAttributes result = source;
            result.Should().BeNull();
        }

        [Test, AutoData]
        public void ImplicitOperator_SourceIsNotNull_ReturnsMappedObject(long id, string name)
        {
            var source = new EntityAttributes { AttributeId = id, AttributeName = name };
            ModelAttributes result = source;
            result.Should().NotBeNull();
            result.AttributeId.Should().Be(id);
            result.AttributeName.Should().Be(name);
        }
    }
}
