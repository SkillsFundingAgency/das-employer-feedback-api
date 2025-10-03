using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAccountsBatch
{
    [TestFixture]
    public class GetAccountsBatchQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var accountIds = new List<long> { 1, 2, 3, 4, 5 };
            var result = new GetAccountsBatchQueryResult
            {
                AccountIds = accountIds
            };

            result.AccountIds.Should().BeEquivalentTo(accountIds);
        }

        [Test]
        public void Constructor_ShouldInitializeEmptyList()
        {
            var result = new GetAccountsBatchQueryResult();

            result.AccountIds.Should().NotBeNull();
            result.AccountIds.Should().BeEmpty();
        }
    }
}