using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAccountsBatch
{
    [TestFixture]
    public class GetAccountsBatchQueryTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var batchSize = 10;
            var query = new GetAccountsBatchQuery
            {
                BatchSize = batchSize
            };

            query.BatchSize.Should().Be(batchSize);
        }
    }
}