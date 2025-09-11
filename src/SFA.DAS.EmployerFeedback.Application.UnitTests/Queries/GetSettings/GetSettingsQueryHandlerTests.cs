using SFA.DAS.EmployerFeedback.Application.Queries.GetSettings;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using Moq;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetSettings
{
    public class GetSettingsQueryHandlerTests
    {
        [Test, AutoData]
        public async Task Handle_ReturnsMappedSettings(List<Domain.Entities.Settings> entities)
        {
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetAll()).ReturnsAsync(entities);
            var handler = new GetSettingsQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetSettingsQuery(), CancellationToken.None);
            result.Settings.Should().BeEquivalentTo(entities.Select(e => new Settings { Name = e.Name, Value = e.Value }));
            mockContext.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoEntitiesExist()
        {
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetAll()).ReturnsAsync(new List<Domain.Entities.Settings>());
            var handler = new GetSettingsQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetSettingsQuery(), CancellationToken.None);
            result.Settings.Should().BeEmpty();
        }

        [Test]
        public async Task Handle_MapsAllPropertiesCorrectly()
        {
            var entities = new List<Domain.Entities.Settings>
            {
                new Domain.Entities.Settings { Name = "Name1", Value = "Value1" },
                new Domain.Entities.Settings { Name = "Name2", Value = "Value2" }
            };
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetAll()).ReturnsAsync(entities);
            var handler = new GetSettingsQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetSettingsQuery(), CancellationToken.None);
            result.Settings.Should().HaveCount(2);
            result.Settings[0].Name.Should().Be("Name1");
            result.Settings[0].Value.Should().Be("Value1");
            result.Settings[1].Name.Should().Be("Name2");
            result.Settings[1].Value.Should().Be("Value2");
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetAll()).ThrowsAsync(new System.Exception("DB error"));
            var handler = new GetSettingsQueryHandler(mockContext.Object);
            Assert.ThrowsAsync<System.Exception>(async () =>
                await handler.Handle(new GetSettingsQuery(), CancellationToken.None));
        }
    }
}
