using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using SFA.DAS.EmployerFeedback.Domain.Models;
using FluentAssertions;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAttributes
{
    public class GetAttributesQueryResultTests
    {
        [Test, AutoData]
        public void Properties_SetAndGet_ShouldReturnExpectedValues(List<Attributes> attributes)
        {
            var result = new GetAttributesQueryResult { Attributes = attributes };
            result.Attributes.Should().BeEquivalentTo(attributes);
        }
    }
}
