using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using NUnit.Framework;
using FluentAssertions;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAttributes
{
    public class GetAttributesQueryTests
    {
        [Test]
        public void CanInstantiate_GetAttributesQuery()
        {
            var query = new GetAttributesQuery();
            query.Should().NotBeNull();
        }
    }
}
