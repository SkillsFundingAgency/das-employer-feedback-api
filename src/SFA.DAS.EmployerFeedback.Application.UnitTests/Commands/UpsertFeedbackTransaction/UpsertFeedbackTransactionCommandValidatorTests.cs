using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertFeedbackTransaction
{
    [TestFixture]
    public class UpsertFeedbackTransactionCommandValidatorTests
    {
        private UpsertFeedbackTransactionCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpsertFeedbackTransactionCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_AccountId_Is_Zero()
        {
            var command = new UpsertFeedbackTransactionCommand { AccountId = 0 };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "AccountId");
        }

        [Test]
        public void Should_Have_Error_When_AccountId_Is_Negative()
        {
            var command = new UpsertFeedbackTransactionCommand { AccountId = -1 };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "AccountId");
        }

        [Test]
        public void Should_Be_Valid_When_AccountId_Is_Positive_And_Arrays_Are_Empty()
        {
            var command = new UpsertFeedbackTransactionCommand 
            { 
                AccountId = 1,
                Active = new List<ProviderCourseDto>(),
                Completed = new List<ProviderCourseDto>(),
                NewStart = new List<ProviderCourseDto>()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_Have_Error_When_Active_Item_Has_Invalid_Ukprn()
        {
            var command = new UpsertFeedbackTransactionCommand 
            { 
                AccountId = 1,
                Active = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = 0, CourseCode = "123" }
                }
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName.Contains("Active[0].Ukprn"));
        }

        [Test]
        public void Should_Have_Error_When_Active_Item_Has_Empty_CourseCode()
        {
            var command = new UpsertFeedbackTransactionCommand 
            { 
                AccountId = 1,
                Active = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = 12345, CourseCode = "" }
                }
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName.Contains("Active[0].CourseCode"));
        }

        [Test]
        public void Should_Have_Error_When_Completed_Item_Has_Invalid_Data()
        {
            var command = new UpsertFeedbackTransactionCommand 
            { 
                AccountId = 1,
                Completed = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = -1, CourseCode = null }
                }
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName.Contains("Completed[0].Ukprn"));
            result.Errors.Should().Contain(x => x.PropertyName.Contains("Completed[0].CourseCode"));
        }

        [Test]
        public void Should_Have_Error_When_NewStart_Item_Has_Invalid_Data()
        {
            var command = new UpsertFeedbackTransactionCommand 
            { 
                AccountId = 1,
                NewStart = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = 0, CourseCode = "" }
                }
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName.Contains("NewStart[0].Ukprn"));
            result.Errors.Should().Contain(x => x.PropertyName.Contains("NewStart[0].CourseCode"));
        }

        [Test]
        public void Should_Be_Valid_When_All_Arrays_Have_Valid_Data()
        {
            var command = new UpsertFeedbackTransactionCommand 
            { 
                AccountId = 123,
                Active = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = 12345, CourseCode = "123" }
                },
                Completed = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = 67890, CourseCode = "456" }
                },
                NewStart = new List<ProviderCourseDto> 
                { 
                    new ProviderCourseDto { Ukprn = 11111, CourseCode = "789" }
                }
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}