using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetFeedbackTransactionsBatch
{
    [TestFixture]
    public class GetFeedbackTransactionsBatchQueryTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var batchSize = 10;
            var query = new GetFeedbackTransactionsBatchQuery
            {
                BatchSize = batchSize
            };

            query.BatchSize.Should().Be(batchSize);
        }
    }
}