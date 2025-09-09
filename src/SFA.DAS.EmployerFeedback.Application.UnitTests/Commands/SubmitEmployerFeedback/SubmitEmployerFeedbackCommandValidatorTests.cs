using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using System;
using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Application.Models;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.SubmitEmployerFeedback
{
    public class SubmitEmployerFeedbackCommandValidatorTests
    {
        private SubmitEmployerFeedbackCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new SubmitEmployerFeedbackCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_UserRef_Is_Empty()
        {
            var command = new SubmitEmployerFeedbackCommand { UserRef = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.UserRef);
        }

        [Test]
        public void Should_Have_Error_When_Ukprn_Is_Zero()
        {
            var command = new SubmitEmployerFeedbackCommand { Ukprn = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Ukprn);
        }

        [Test]
        public void Should_Have_Error_When_AccountId_Is_Zero()
        {
            var command = new SubmitEmployerFeedbackCommand { AccountId = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AccountId);
        }

        [Test]
        public void Should_Have_Error_When_ProviderRating_Is_Invalid()
        {
            var command = new SubmitEmployerFeedbackCommand { ProviderRating = (OverallRating)999 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ProviderRating);
        }

        [Test]
        public void Should_Have_Error_When_FeedbackSource_Is_Out_Of_Range()
        {
            var command = new SubmitEmployerFeedbackCommand { FeedbackSource = 0 };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FeedbackSource);
        }

        [Test]
        public void Should_Have_Error_When_ProviderAttributes_Invalid()
        {
            var command = new SubmitEmployerFeedbackCommand
            {
                ProviderAttributes = new List<ProviderAttributeDto>
                {
                    new ProviderAttributeDto { AttributeId = 0, AttributeValue = 2 }
                }
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor("ProviderAttributes[0].AttributeId");
            result.ShouldHaveValidationErrorFor("ProviderAttributes[0].AttributeValue");
        }

        [Test]
        public void Should_Not_Have_Error_When_Valid()
        {
            var command = new SubmitEmployerFeedbackCommand
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 123,
                AccountId = 456,
                ProviderRating = OverallRating.Excellent,
                FeedbackSource = 1,
                ProviderAttributes = new List<ProviderAttributeDto>
                {
                    new ProviderAttributeDto { AttributeId = 1, AttributeValue = 1 }
                }
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
