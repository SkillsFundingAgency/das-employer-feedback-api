using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.GenerateFeedbackSummaries;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.GenerateFeedbackSummaries
{
    public class GenerateFeedbackSummariesCommandTests
    {
        [Test]
        public void CanInstantiate_GenerateFeedbackSummariesCommand()
        {
            var command = new GenerateFeedbackSummariesCommand();
            Assert.That(command, Is.Not.Null);
        }
    }
}
