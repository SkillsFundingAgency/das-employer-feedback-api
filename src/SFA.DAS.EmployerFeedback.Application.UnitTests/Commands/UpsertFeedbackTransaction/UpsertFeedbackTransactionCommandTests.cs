using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertFeedbackTransaction
{
    [TestFixture]
    public class UpsertFeedbackTransactionCommandTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var active = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 1, CourseCode = "A" } };
            var completed = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 2, CourseCode = "B" } };
            var newStart = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 3, CourseCode = "C" } };

            var command = new UpsertFeedbackTransactionCommand
            {
                AccountId = 123,
                Active = active,
                Completed = completed,
                NewStart = newStart
            };

            Assert.That(command.AccountId, Is.EqualTo(123));
            Assert.That(command.Active, Is.EqualTo(active));
            Assert.That(command.Completed, Is.EqualTo(completed));
            Assert.That(command.NewStart, Is.EqualTo(newStart));
        }

        [Test]
        public void ImplicitOperator_FromRequest_ShouldMapProperties()
        {
            var active = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 10, CourseCode = "X" } };
            var completed = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 20, CourseCode = "Y" } };
            var newStart = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 30, CourseCode = "Z" } };

            var request = new UpsertFeedbackTransactionRequest
            {
                Active = active,
                Completed = completed,
                NewStart = newStart
            };

            UpsertFeedbackTransactionCommand command = request;

            Assert.That(command.Active, Is.EqualTo(active));
            Assert.That(command.Completed, Is.EqualTo(completed));
            Assert.That(command.NewStart, Is.EqualTo(newStart));
        }
    }
}
