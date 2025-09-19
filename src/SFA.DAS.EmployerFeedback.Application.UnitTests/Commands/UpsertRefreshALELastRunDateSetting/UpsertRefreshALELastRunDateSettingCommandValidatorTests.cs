using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertRefreshALELastRunDateSetting;
using System;
using System.Globalization;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertRefreshALELastRunDateSetting
{
    [TestFixture]
    public class UpsertRefreshALELastRunDateSettingCommandValidatorTests
    {
        private UpsertRefreshALELastRunDateSettingCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpsertRefreshALELastRunDateSettingCommandValidator();
        }

        [Test]
        public void Should_Not_Have_Error_When_Value_Is_Null()
        {
            var command = new UpsertRefreshALELastRunDateSettingCommand { Value = null };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Value);
        }

        [Test]
        public void Should_Not_Have_Error_When_Value_Is_Valid_DateTime()
        {
            var command = new UpsertRefreshALELastRunDateSettingCommand { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Value);
        }

        [Test]
        public void Should_Not_Have_Error_When_Value_Is_Valid_Specific_Date()
        {
            var command = new UpsertRefreshALELastRunDateSettingCommand { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Value);
        }

        [Test]
        public void Should_Have_Error_When_Value_Is_Invalid_Date()
        {
            var command = new UpsertRefreshALELastRunDateSettingCommand { Value = "not-a-date" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Value);
        }
    }
}
