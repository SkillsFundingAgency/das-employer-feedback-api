using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetFeedbackTransactionsBatch
{
    [TestFixture]
    public class GetFeedbackTransactionsBatchQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var feedbackTransactionIds = new List<long> { 1, 2, 3, 4, 5 };
            var result = new GetFeedbackTransactionsBatchQueryResult
            {
                FeedbackTransactions = feedbackTransactionIds
            };

            result.FeedbackTransactions.Should().BeEquivalentTo(feedbackTransactionIds);
        }

        [Test]
        public void Constructor_ShouldInitializeEmptyList()
        {
            var result = new GetFeedbackTransactionsBatchQueryResult();

            result.FeedbackTransactions.Should().NotBeNull();
            result.FeedbackTransactions.Should().BeEmpty();
        }

        [Test]
        public void Properties_SetEmptyList_ShouldReturnEmptyList()
        {
            var emptyList = new List<long>();
            var result = new GetFeedbackTransactionsBatchQueryResult
            {
                FeedbackTransactions = emptyList
            };

            result.FeedbackTransactions.Should().NotBeNull();
            result.FeedbackTransactions.Should().BeEmpty();
        }
    }
}