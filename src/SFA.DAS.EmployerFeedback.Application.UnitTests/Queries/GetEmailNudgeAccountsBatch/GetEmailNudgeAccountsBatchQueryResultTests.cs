using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmailNudgeAccountsBatch;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmailNudgeAccountsBatch
{
    [TestFixture]
    public class GetEmailNudgeAccountsBatchQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var accountIds = new List<long> { 1, 2, 3, 4, 5 };
            var result = new GetEmailNudgeAccountsBatchQueryResult
            {
                AccountIds = accountIds
            };

            result.AccountIds.Should().BeEquivalentTo(accountIds);
        }

        [Test]
        public void Constructor_ShouldInitializeEmptyList()
        {
            var result = new GetEmailNudgeAccountsBatchQueryResult();

            result.AccountIds.Should().NotBeNull();
            result.AccountIds.Should().BeEmpty();
        }
    }
}