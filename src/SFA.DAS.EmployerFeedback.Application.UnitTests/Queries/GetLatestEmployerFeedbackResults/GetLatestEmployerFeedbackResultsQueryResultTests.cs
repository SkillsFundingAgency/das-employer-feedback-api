using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetLatestEmployerFeedbackResults
{
    public class GetLatestEmployerFeedbackQueryResultsTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(List<EmployerFeedbackItem> employerFeedbacks)
        {
            var result = new GetLatestEmployerFeedbackResultsQueryResult { EmployerFeedbacks = employerFeedbacks };
            result.EmployerFeedbacks.Should().BeEquivalentTo(employerFeedbacks);
        }
    }
}
