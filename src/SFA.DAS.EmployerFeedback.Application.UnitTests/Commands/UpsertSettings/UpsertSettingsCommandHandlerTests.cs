using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertSettings
{
    [TestFixture]
    public class UpsertSettingsCommandHandlerTests
    {
        private Mock<ISettingsContext> _settingsContext;
        private Mock<ILogger<UpsertSettingsCommandHandler>> _logger;
        private UpsertSettingsCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _settingsContext = new Mock<ISettingsContext>();
            _logger = new Mock<ILogger<UpsertSettingsCommandHandler>>();
            _handler = new UpsertSettingsCommandHandler(_settingsContext.Object, _logger.Object);
        }

        [Test]
        public async Task Handle_Should_Add_New_Setting_If_Not_Exists()
        {
            var command = new UpsertSettingsCommand
            {
                Settings = new List<SettingDto> { new SettingDto { Name = "Test", Value = DateTime.UtcNow } }
            };
            _settingsContext.Setup(x => x.GetByNameAsync("Test", It.IsAny<CancellationToken>())).ReturnsAsync((Settings)null);
            _settingsContext.Setup(x => x.Add(It.IsAny<Settings>())).Verifiable();
            _settingsContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);
            _settingsContext.Verify(x => x.Add(It.IsAny<Settings>()), Times.Once);
            _settingsContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Update_Setting_If_Exists()
        {
            var existing = new Settings { Name = "Test", Value = DateTime.UtcNow };
            var command = new UpsertSettingsCommand
            {
                Settings = new List<SettingDto> { new SettingDto { Name = "Test", Value = DateTime.UtcNow } }
            };
            _settingsContext.Setup(x => x.GetByNameAsync("Test", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            _settingsContext.Setup(x => x.Update(existing)).Verifiable();
            _settingsContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _handler.Handle(command, CancellationToken.None);
            _settingsContext.Verify(x => x.Update(existing), Times.Once);
            _settingsContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_Should_Log_And_Throw_On_Exception()
        {
            var command = new UpsertSettingsCommand
            {
                Settings = new List<SettingDto> { new SettingDto { Name = "Test", Value = DateTime.UtcNow } }
            };
            _settingsContext.Setup(x => x.GetByNameAsync("Test", It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));

            Assert.ThrowsAsync<System.Exception>(async () => await _handler.Handle(command, CancellationToken.None));
            _logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error saving settings")),
                It.IsAny<System.Exception>(),
                It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()), Times.Once);
        }
    }
}
