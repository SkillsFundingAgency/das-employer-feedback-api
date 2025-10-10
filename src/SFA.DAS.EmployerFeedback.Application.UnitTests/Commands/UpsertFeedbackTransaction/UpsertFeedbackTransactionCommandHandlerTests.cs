using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertFeedbackTransaction
{
    [TestFixture]
    public class UpsertFeedbackTransactionCommandHandlerTests
    {
        private Mock<IFeedbackTransactionContext> _mockFeedbackTransactionContext;
        private Mock<IAccountContext> _mockAccountContext;
        private Mock<ILogger<UpsertFeedbackTransactionCommandHandler>> _mockLogger;
        private Mock<IDateTimeHelper> _mockDateTimeHelper;
        private ApplicationSettings _applicationSettings;
        private UpsertFeedbackTransactionCommandHandler _handler;
        private DateTime _fixedDateTime;

        [SetUp]
        public void SetUp()
        {
            _mockFeedbackTransactionContext = new Mock<IFeedbackTransactionContext>();
            _mockAccountContext = new Mock<IAccountContext>();
            _mockLogger = new Mock<ILogger<UpsertFeedbackTransactionCommandHandler>>();
            _mockDateTimeHelper = new Mock<IDateTimeHelper>();
            _applicationSettings = new ApplicationSettings { EmailNudgeSendAfterDays = 30 };
            _fixedDateTime = new DateTime(2024, 1, 15, 10, 0, 0);

            _mockDateTimeHelper.Setup(x => x.Now).Returns(_fixedDateTime);

            _handler = new UpsertFeedbackTransactionCommandHandler(
                _mockFeedbackTransactionContext.Object,
                _mockAccountContext.Object,
                _applicationSettings,
                _mockLogger.Object,
                _mockDateTimeHelper.Object);
        }

        [Test, AutoData]
        public async Task Handle_WhenAllArraysAreEmpty_ShouldOnlyUpdateAccountCheckedOn(long accountId)
        {
            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                Active = new List<ProviderCourseDto>(),
                Completed = new List<ProviderCourseDto>(),
                NewStart = new List<ProviderCourseDto>()
            };

            var account = new Account { Id = accountId, AccountName = "Test Account", CheckedOn = _fixedDateTime.AddDays(-10) };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            var originalCheckedOn = account.CheckedOn;
            await _handler.Handle(command, CancellationToken.None);

            _mockFeedbackTransactionContext.Verify(x => x.Add(It.IsAny<FeedbackTransaction>()), Times.Never);
            _mockFeedbackTransactionContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            account.CheckedOn.Should().Be(_fixedDateTime);
            account.CheckedOn.Should().BeAfter(originalCheckedOn.Value);
        }

        [Test, AutoData]
        public async Task Handle_WhenNewStartHasData_ShouldCreateTransactionWithSendAfterNow_AndUpdateAccountCheckedOn(long accountId)
        {
            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                NewStart = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            var account = new Account { Id = accountId, CheckedOn = _fixedDateTime.AddDays(-1) };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeedbackTransaction)null);

            var originalCheckedOn = account.CheckedOn;
            FeedbackTransaction addedTransaction = null;
            _mockFeedbackTransactionContext
                .Setup(x => x.Add(It.IsAny<FeedbackTransaction>()))
                .Callback<FeedbackTransaction>(ft => addedTransaction = ft);

            await _handler.Handle(command, CancellationToken.None);

            addedTransaction.Should().NotBeNull();
            addedTransaction.AccountId.Should().Be(accountId);
            addedTransaction.SendAfter.Should().Be(_fixedDateTime);
            addedTransaction.CreatedOn.Should().Be(_fixedDateTime);
            account.CheckedOn.Should().Be(_fixedDateTime);
            account.CheckedOn.Should().BeAfter(originalCheckedOn.Value);
            _mockFeedbackTransactionContext.Verify(x => x.Add(It.IsAny<FeedbackTransaction>()), Times.Once);
            _mockFeedbackTransactionContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Test, AutoData]
        public async Task Handle_WhenCompletedHasData_ShouldCreateTransactionWithSendAfterEmailNudgeSendAfterDays(long accountId)
        {
            var emailNudgeSendAfterDays = 30;
            _applicationSettings.EmailNudgeSendAfterDays = emailNudgeSendAfterDays;
            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                Completed = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            var account = new Account { Id = accountId };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeedbackTransaction)null);

            FeedbackTransaction addedTransaction = null;
            _mockFeedbackTransactionContext
                .Setup(x => x.Add(It.IsAny<FeedbackTransaction>()))
                .Callback<FeedbackTransaction>(ft => addedTransaction = ft);

            await _handler.Handle(command, CancellationToken.None);

            addedTransaction.Should().NotBeNull();
            addedTransaction.AccountId.Should().Be(accountId);
            addedTransaction.SendAfter.Should().Be(_fixedDateTime.AddDays(emailNudgeSendAfterDays));
            addedTransaction.CreatedOn.Should().Be(_fixedDateTime);
            _mockFeedbackTransactionContext.Verify(x => x.Add(It.IsAny<FeedbackTransaction>()), Times.Once);
            _mockFeedbackTransactionContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, AutoData]
        public async Task Handle_WhenActiveHasData_ShouldCreateTransactionWithSendAfterTwiceEmailNudgeSendAfterDays(long accountId)
        {
            _applicationSettings.EmailNudgeSendAfterDays = 30;
            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                Active = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            var account = new Account { Id = accountId };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeedbackTransaction)null);

            FeedbackTransaction addedTransaction = null;
            _mockFeedbackTransactionContext
                .Setup(x => x.Add(It.IsAny<FeedbackTransaction>()))
                .Callback<FeedbackTransaction>(ft => addedTransaction = ft);

            await _handler.Handle(command, CancellationToken.None);

            addedTransaction.Should().NotBeNull();
            addedTransaction.AccountId.Should().Be(accountId);
            addedTransaction.SendAfter.Should().Be(_fixedDateTime.AddDays(60));
            addedTransaction.CreatedOn.Should().Be(_fixedDateTime);
            _mockFeedbackTransactionContext.Verify(x => x.Add(It.IsAny<FeedbackTransaction>()), Times.Once);
            _mockFeedbackTransactionContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, AutoData]
        public async Task Handle_WhenExistingTransactionNotSent_ShouldUpdateSendAfterIfEarlier(long accountId)
        {
            var existingSendAfter = _fixedDateTime.AddDays(10);
            var existingTransaction = new FeedbackTransaction
            {
                Id = 1,
                AccountId = accountId,
                SendAfter = existingSendAfter,
                SentDate = null
            };

            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                Completed = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            var account = new Account { Id = accountId };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTransaction);

            _applicationSettings.EmailNudgeSendAfterDays = 5;

            await _handler.Handle(command, CancellationToken.None);

            existingTransaction.SendAfter.Should().Be(_fixedDateTime.AddDays(5));
            existingTransaction.SendAfter.Should().BeBefore(existingSendAfter);
            _mockFeedbackTransactionContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Test, AutoData]
        public async Task Handle_WhenExistingTransactionAlreadySent_ShouldCreateNewTransaction(long accountId)
        {
            var existingTransaction = new FeedbackTransaction
            {
                Id = 1,
                AccountId = accountId,
                SendAfter = _fixedDateTime.AddDays(-5),
                SentDate = _fixedDateTime.AddDays(-3)
            };

            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                Completed = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            var account = new Account { Id = accountId };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTransaction);

            await _handler.Handle(command, CancellationToken.None);

            _mockFeedbackTransactionContext.Verify(x => x.Add(
                It.Is<FeedbackTransaction>(ft => ft.AccountId == accountId && ft.SentDate == null && ft.CreatedOn == _fixedDateTime)), Times.Once);
        }

        [Test, AutoData]
        public async Task Handle_WhenExceptionOccurs_ShouldThrowException(long accountId)
        {
            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                NewStart = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
        }

        [Test, AutoData]
        public async Task Handle_ShouldAlwaysUpdateAccountCheckedOn(long accountId)
        {
            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = accountId,
                NewStart = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" } }
            };

            var account = new Account { Id = accountId, CheckedOn = _fixedDateTime.AddDays(-1) };
            _mockAccountContext.Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockFeedbackTransactionContext.Setup(x => x.GetMostRecentByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeedbackTransaction)null);

            var originalCheckedOn = account.CheckedOn;

            await _handler.Handle(command, CancellationToken.None);

            account.CheckedOn.Should().Be(_fixedDateTime);
            account.CheckedOn.Should().BeAfter(originalCheckedOn.Value);
            _mockFeedbackTransactionContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}