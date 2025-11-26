using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction;
using System;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpdateFeedbackTransaction
{
    [TestFixture]
    public class UpdateFeedbackTransactionCommandValidatorTests
    {
        private UpdateFeedbackTransactionCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpdateFeedbackTransactionCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Id_Is_Zero()
        {
            var command = new UpdateFeedbackTransactionCommand { Id = 0 };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Id");
        }

        [Test]
        public void Should_Have_Error_When_Id_Is_Negative()
        {
            var command = new UpdateFeedbackTransactionCommand { Id = -1 };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Id");
        }

        [Test]
        public void Should_Have_Error_When_TemplateId_Is_Empty()
        {
            var command = new UpdateFeedbackTransactionCommand
            {
                Id = 1,
                TemplateId = Guid.Empty,
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "TemplateId");
        }

        [Test]
        public void Should_Have_Error_When_SentCount_Is_Negative()
        {
            var command = new UpdateFeedbackTransactionCommand
            {
                Id = 1,
                TemplateId = Guid.NewGuid(),
                SentCount = -1,
                SentDate = DateTime.UtcNow
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "SentCount");
        }

        [Test]
        public void Should_Have_Error_When_SentDate_Is_Empty()
        {
            var command = new UpdateFeedbackTransactionCommand
            {
                Id = 1,
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = default(DateTime)
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "SentDate");
        }

        [Test]
        public void Should_Be_Valid_When_All_Properties_Are_Correct()
        {
            var command = new UpdateFeedbackTransactionCommand
            {
                Id = 1,
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}