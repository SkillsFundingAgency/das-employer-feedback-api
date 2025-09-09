using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.SubmitEmployerFeedback
{
    [TestFixture]
    public class SubmitEmployerFeedbackCommandHandlerTests
    {
        private Mock<IEmployerFeedbackContext> _employerFeedbackContext;
        private Mock<IEmployerFeedbackResultContext> _employerFeedbackResultContext;
        private Mock<IProviderAttributeContext> _providerAttributeContext;
        private Mock<ILogger<SubmitEmployerFeedbackCommandHandler>> _logger;
        private SubmitEmployerFeedbackCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _employerFeedbackContext = new Mock<IEmployerFeedbackContext>();
            _employerFeedbackResultContext = new Mock<IEmployerFeedbackResultContext>();
            _providerAttributeContext = new Mock<IProviderAttributeContext>();
            _logger = new Mock<ILogger<SubmitEmployerFeedbackCommandHandler>>();
            _handler = new SubmitEmployerFeedbackCommandHandler(
                _employerFeedbackContext.Object,
                _employerFeedbackResultContext.Object,
                _providerAttributeContext.Object,
                _logger.Object);
        }

        [Test]
        public async Task Handle_Should_Add_New_EmployerFeedback_If_Not_Exists()
        {
            var command = new SubmitEmployerFeedbackCommand
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 1,
                AccountId = 2,
                ProviderRating = OverallRating.Good,
                FeedbackSource = 1,
                ProviderAttributes = new List<ProviderAttributeDto>()
            };
            var feedback = new Domain.Entities.EmployerFeedback { FeedbackId = 123, UserRef = command.UserRef, Ukprn = command.Ukprn, AccountId = command.AccountId, IsActive = true };
            _employerFeedbackContext.Setup(x => x.GetByUserUkprnAccountAsync(command.UserRef, command.Ukprn, command.AccountId, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.EmployerFeedback)null);
            _employerFeedbackContext.Setup(x => x.Add(It.IsAny<Domain.Entities.EmployerFeedback>())).Callback<Domain.Entities.EmployerFeedback>(f => f.FeedbackId = feedback.FeedbackId);
            _employerFeedbackContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            _employerFeedbackContext.Verify(x => x.Add(It.IsAny<Domain.Entities.EmployerFeedback>()), Times.Once);
            _employerFeedbackResultContext.Verify(x => x.Add(It.IsAny<EmployerFeedbackResult>()), Times.Once);
            _employerFeedbackContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.EmployerFeedbackId, Is.EqualTo(feedback.FeedbackId));
        }

        [Test]
        public async Task Handle_Should_Update_EmployerFeedback_If_Inactive()
        {
            var command = new SubmitEmployerFeedbackCommand
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 1,
                AccountId = 2,
                ProviderRating = OverallRating.Good,
                FeedbackSource = 1,
                ProviderAttributes = new List<ProviderAttributeDto>()
            };
            var feedback = new Domain.Entities.EmployerFeedback { FeedbackId = 456, UserRef = command.UserRef, Ukprn = command.Ukprn, AccountId = command.AccountId, IsActive = false };
            _employerFeedbackContext.Setup(x => x.GetByUserUkprnAccountAsync(command.UserRef, command.Ukprn, command.AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            _employerFeedbackContext.Setup(x => x.Update(feedback)).Callback(() => feedback.IsActive = true);
            _employerFeedbackContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            _employerFeedbackContext.Verify(x => x.Update(It.IsAny<Domain.Entities.EmployerFeedback>()), Times.Once);
            _employerFeedbackResultContext.Verify(x => x.Add(It.IsAny<EmployerFeedbackResult>()), Times.Once);
            _employerFeedbackContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.EmployerFeedbackId, Is.EqualTo(feedback.FeedbackId));
        }

        [Test]
        public async Task Handle_Should_Add_ProviderAttributes_When_Provided()
        {
            var command = new SubmitEmployerFeedbackCommand
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 1,
                AccountId = 2,
                ProviderRating = OverallRating.Good,
                FeedbackSource = 1,
                ProviderAttributes = new List<ProviderAttributeDto>
                {
                    new ProviderAttributeDto { AttributeId = 1, AttributeValue = 1 },
                    new ProviderAttributeDto { AttributeId = 2, AttributeValue = -1 }
                }
            };
            var feedback = new Domain.Entities.EmployerFeedback { FeedbackId = 789, UserRef = command.UserRef, Ukprn = command.Ukprn, AccountId = command.AccountId, IsActive = true };
            _employerFeedbackContext.Setup(x => x.GetByUserUkprnAccountAsync(command.UserRef, command.Ukprn, command.AccountId, It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            _employerFeedbackContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var providerAttributeCalls = 0;
            _providerAttributeContext.Setup(x => x.Add(It.IsAny<ProviderAttribute>())).Callback(() => providerAttributeCalls++);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(providerAttributeCalls, Is.EqualTo(2));
            _employerFeedbackResultContext.Verify(x => x.Add(It.IsAny<EmployerFeedbackResult>()), Times.Once);
            _employerFeedbackContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Handle_Should_Log_And_Throw_On_Exception()
        {
            var command = new SubmitEmployerFeedbackCommand
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 1,
                AccountId = 2,
                ProviderRating = OverallRating.Good,
                FeedbackSource = 1,
                ProviderAttributes = new List<ProviderAttributeDto>()
            };
            _employerFeedbackContext.Setup(x => x.GetByUserUkprnAccountAsync(command.UserRef, command.Ukprn, command.AccountId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(command, CancellationToken.None));
            _logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error submitting employer feedback")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
