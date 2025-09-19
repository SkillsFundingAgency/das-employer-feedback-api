using SFA.DAS.EmployerFeedback.Application.Queries.GetRefreshALELastRunDateSetting;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using Moq;
using FluentAssertions;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetRefreshALELastRunDateSetting
{
    public class GetRefreshALELastRunDateSettingQueryHandlerTests
    {
        private const string SettingName = "RefreshALELastRunDate";

        [Test]
        public async Task Handle_ReturnsMappedSetting()
        {
            var nowString = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var entity = new Domain.Entities.Settings { Name = SettingName, Value = nowString };
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetByNameAsync(SettingName, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            var handler = new GetRefreshALELastRunDateSettingQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetRefreshALELastRunDateSettingQuery(), CancellationToken.None);
            result.Value.Should().Be(nowString);
            mockContext.Verify(x => x.GetByNameAsync(SettingName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsNullValue_WhenNoEntityExists()
        {
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetByNameAsync(SettingName, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Settings)null);
            var handler = new GetRefreshALELastRunDateSettingQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetRefreshALELastRunDateSettingQuery(), CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var mockContext = new Mock<ISettingsContext>();
            mockContext.Setup(x => x.GetByNameAsync(SettingName, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));
            var handler = new GetRefreshALELastRunDateSettingQueryHandler(mockContext.Object);
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await handler.Handle(new GetRefreshALELastRunDateSettingQuery(), CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("DB error"));
        }
    }
}
