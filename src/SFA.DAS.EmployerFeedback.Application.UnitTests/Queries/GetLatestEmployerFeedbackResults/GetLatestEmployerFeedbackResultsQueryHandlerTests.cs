using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetLatestEmployerFeedbackResults
{
    public class GetLatestEmployerFeedbackQueryHandlerTests
    {
        [Test, AutoData]
        public async Task And_NoResults_Then_NullReturned(
         [Frozen] Mock<IEmployerFeedbackTargetContext> context)
        {
            // Arrange
            var accountId = 123L;
            var userRef = Guid.NewGuid();

            context.Setup(c => c.GetLatestResultsPerAccount(accountId, userRef, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<LatestEmployerFeedbackResults>());

            var handler = new GetLatestEmployerFeedbackResultsQueryHandler(context.Object);
            var query = new GetLatestEmployerFeedbackResultsQuery { AccountId = accountId, UserRef = userRef };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test, AutoData]
        public async Task And_ResultsExist_Then_MapsToDto_WithResultOnlyWhenCompletedDatePresent(
            [Frozen] Mock<IEmployerFeedbackTargetContext> context)
        {
            // Arrange
            var accountId = 555L;
            var userRef = Guid.NewGuid();

            var dt = new DateTime(2025, 05, 01, 12, 0, 0, DateTimeKind.Utc);

            var source = new List<LatestEmployerFeedbackResults>
            {
                new LatestEmployerFeedbackResults
                {
                    AccountId = accountId,
                    AccountName = "ACME Ltd",
                    Ukprn = 11111111,
                    DateTimeCompleted = dt,
                    ProviderRating = "Good",
                    FeedbackSource = 1
                },
                new LatestEmployerFeedbackResults
                {
                    AccountId = accountId,
                    AccountName = "ACME Ltd",
                    Ukprn = 22222222,
                    DateTimeCompleted = null,
                    ProviderRating = null,
                    FeedbackSource = null
                }
            };

            context.Setup(c => c.GetLatestResultsPerAccount(accountId, userRef, It.IsAny<CancellationToken>()))
                .ReturnsAsync(source);

            var handler = new GetLatestEmployerFeedbackResultsQueryHandler(context.Object);
            var query = new GetLatestEmployerFeedbackResultsQuery { AccountId = accountId, UserRef = userRef };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.AccountId.Should().Be(accountId);
            result.AccountName.Should().Be("ACME Ltd");

            result.EmployerFeedbacks.Should().HaveCount(2);

            var first = result.EmployerFeedbacks.Single(x => x.Ukprn == 11111111);
            first.Result.Should().NotBeNull();
            first.Result!.DateTimeCompleted.Should().Be(dt);
            first.Result.ProviderRating.Should().Be("Good");
            first.Result.FeedbackSource.Should().Be(1);

            var second = result.EmployerFeedbacks.Single(x => x.Ukprn == 22222222);
            second.Result.Should().BeNull();
        }

        [Test, AutoData]
        public async Task And_ContextReturnsOrderedByUkprn_Then_HandlerPreservesOrder(
            [Frozen] Mock<IEmployerFeedbackTargetContext> context)
        {
            // Arrange
            var accountId = 77L;
            var userRef = Guid.NewGuid();

            var source = new List<LatestEmployerFeedbackResults>
            {
                new LatestEmployerFeedbackResults
                {
                    AccountId = accountId, AccountName = "Widgets Ltd",
                    Ukprn = 10000002, DateTimeCompleted = null
                },
                new LatestEmployerFeedbackResults
                {
                    AccountId = accountId, AccountName = "Widgets Ltd",
                    Ukprn = 10000005, DateTimeCompleted = new DateTime(2025, 07, 01, 0, 0, 0, DateTimeKind.Utc),
                    ProviderRating = "Excellent",
                    FeedbackSource = 1
                },
                new LatestEmployerFeedbackResults
                {
                    AccountId = accountId, AccountName = "Widgets Ltd",
                    Ukprn = 10000009, DateTimeCompleted = null
                }
            }
            .OrderBy(x => x.Ukprn) // force order on arrange so it can be checked on assert
            .ToList();

            context.Setup(c => c.GetLatestResultsPerAccount(accountId, userRef, It.IsAny<CancellationToken>()))
                .ReturnsAsync(source);

            var handler = new GetLatestEmployerFeedbackResultsQueryHandler(context.Object);
            var query = new GetLatestEmployerFeedbackResultsQuery { AccountId = accountId, UserRef = userRef };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.EmployerFeedbacks.Select(x => x.Ukprn)
                .Should().ContainInOrder(10000002, 10000005, 10000009);
        }
    }
}