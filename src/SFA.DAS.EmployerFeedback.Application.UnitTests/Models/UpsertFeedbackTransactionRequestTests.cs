using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Models
{
    [TestFixture]
    public class UpsertFeedbackTransactionRequestTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var active = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 1, CourseCode = "A" } };
            var completed = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 2, CourseCode = "B" } };
            var newStart = new List<ProviderCourseDto> { new ProviderCourseDto { Ukprn = 3, CourseCode = "C" } };

            var request = new UpsertFeedbackTransactionRequest
            {
                Active = active,
                Completed = completed,
                NewStart = newStart
            };

            Assert.That(request.Active, Is.EqualTo(active));
            Assert.That(request.Completed, Is.EqualTo(completed));
            Assert.That(request.NewStart, Is.EqualTo(newStart));
        }

        [Test]
        public void Default_Constructor_ShouldInitializeEmptyLists()
        {
            var request = new UpsertFeedbackTransactionRequest();
            Assert.That(request.Active, Is.Not.Null);
            Assert.That(request.Completed, Is.Not.Null);
            Assert.That(request.NewStart, Is.Not.Null);
            Assert.That(request.Active, Is.Empty);
            Assert.That(request.Completed, Is.Empty);
            Assert.That(request.NewStart, Is.Empty);
        }
    }
}
