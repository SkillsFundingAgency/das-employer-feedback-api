using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertRefreshALELastRunDateSetting;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertRefreshALELastRunDateSetting
{
    [TestFixture]
    public class UpsertRefreshALELastRunDateSettingCommandHandlerTests
    {
        private Mock<ISettingsContext> _settingsContext;
        private Mock<ILogger<UpsertRefreshALELastRunDateSettingCommandHandler>> _logger;
        private UpsertRefreshALELastRunDateSettingCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _settingsContext = new Mock<ISettingsContext>();
            _logger = new Mock<ILogger<UpsertRefreshALELastRunDateSettingCommandHandler>>();
            _handler = new UpsertRefreshALELastRunDateSettingCommandHandler(_settingsContext.Object, _logger.Object);
        }

        [Test]
        public async Task Handle_Should_Add_New_Setting_If_Not_Exists()
        {
            var value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var name = SettingType.RefreshALELastRunDate.ToString();
            var command = new UpsertRefreshALELastRunDateSettingCommand
            {
                Value = value
            };
            _settingsContext.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>())).ReturnsAsync((Settings)null);
            _settingsContext.Setup(x => x.Add(It.IsAny<Settings>())).Verifiable();
            _settingsContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);
            _settingsContext.Verify(x => x.Add(It.Is<Settings>(s => s.Name == name && s.Value == value)), Times.Once);
            _settingsContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Update_Setting_If_Exists()
        {
            var value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var name = SettingType.RefreshALELastRunDate.ToString();
            var command = new UpsertRefreshALELastRunDateSettingCommand
            {
                Value = value
            };
            var existing = new Settings { Name = name, Value = null };
            _settingsContext.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _settingsContext.Setup(x => x.Update(existing)).Verifiable();
            _settingsContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);
            _settingsContext.Verify(x => x.Update(existing), Times.Once);
            _settingsContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(command.Value, existing.Value);
        }

        [Test]
        public void Handle_Should_Log_And_Throw_On_Exception()
        {
            var name = SettingType.RefreshALELastRunDate.ToString();
            var command = new UpsertRefreshALELastRunDateSettingCommand
            {
                Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
            };
            _settingsContext.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(command, CancellationToken.None));
            _logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error saving settings")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
