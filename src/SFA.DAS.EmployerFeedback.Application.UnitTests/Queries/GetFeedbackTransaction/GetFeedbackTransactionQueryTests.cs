using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction;
using AutoFixture.NUnit3;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetFeedbackTransaction
{
    [TestFixture]
    public class GetFeedbackTransactionQueryTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(long id)
        {
            var query = new GetFeedbackTransactionQuery
            {
                Id = id
            };

            query.Id.Should().Be(id);
        }
    }
}