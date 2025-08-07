using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using Moq;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAttributes
{
    public class GetAttributesQueryHandlerTests
    {
        [Test, AutoData]
        public async Task Handle_ReturnsMappedAttributes(List<Attributes> entities)
        {
            var mockContext = new Mock<IAttributeEntityContext>();
            mockContext.Setup(x => x.GetAll()).ReturnsAsync(entities);
            var handler = new GetAttributesQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetAttributesQuery(), CancellationToken.None);
            result.Attributes.Should().BeEquivalentTo(entities.Select(e => (Domain.Models.Attributes)e));
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoEntitiesExist()
        {
            var mockContext = new Mock<IAttributeEntityContext>();
            mockContext.Setup(x => x.GetAll()).ReturnsAsync(new List<Attributes>());
            var handler = new GetAttributesQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetAttributesQuery(), CancellationToken.None);
            result.Attributes.Should().BeEmpty();
        }

        [Test]
        public async Task Handle_MapsAllPropertiesCorrectly()
        {
            var entities = new List<Attributes>
            {
                new Attributes { AttributeId = 1, AttributeName = "Name1" },
                new Attributes { AttributeId = 2, AttributeName = "Name2" }
            };
            var mockContext = new Mock<IAttributeEntityContext>();
            mockContext.Setup(x => x.GetAll()).ReturnsAsync(entities);
            var handler = new GetAttributesQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetAttributesQuery(), CancellationToken.None);
            result.Attributes.Should().HaveCount(2);
            result.Attributes[0].AttributeId.Should().Be(1);
            result.Attributes[0].AttributeName.Should().Be("Name1");
            result.Attributes[1].AttributeId.Should().Be(2);
            result.Attributes[1].AttributeName.Should().Be("Name2");
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var mockContext = new Mock<IAttributeEntityContext>();
            mockContext.Setup(x => x.GetAll()).ThrowsAsync(new System.Exception("DB error"));
            var handler = new GetAttributesQueryHandler(mockContext.Object);
            Assert.ThrowsAsync<System.Exception>(async () =>
                await handler.Handle(new GetAttributesQuery(), CancellationToken.None));
        }
    }
}
