using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAttributes
{
    public class GetAttributesQueryTests
    {
        [Test]
        public void CanInstantiate_GetAttributesQuery()
        {
            var query = new GetAttributesQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
