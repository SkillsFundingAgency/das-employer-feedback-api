using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmailNudgeAccountsBatch;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmailNudgeAccountsBatch
{
    [TestFixture]
    public class GetEmailNudgeAccountsBatchQueryTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var batchSize = 10;
            var query = new GetEmailNudgeAccountsBatchQuery
            {
                BatchSize = batchSize
            };

            query.BatchSize.Should().Be(batchSize);
        }
    }
}