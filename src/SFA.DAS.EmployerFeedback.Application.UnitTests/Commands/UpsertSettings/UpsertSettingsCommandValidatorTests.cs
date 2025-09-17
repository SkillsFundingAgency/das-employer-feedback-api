using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertSettings
{
    [TestFixture]
    public class UpsertSettingsCommandValidatorTests
    {
        private UpsertSettingsCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpsertSettingsCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Settings_Is_Null()
        {
            var command = new UpsertSettingsCommand { Settings = null };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Settings);
        }

        [Test]
        public void Should_Have_Error_When_Settings_Is_Empty()
        {
            var command = new UpsertSettingsCommand { Settings = new List<SettingDto>() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Settings);
        }

        [Test]
        public void Should_Have_Error_When_Name_Is_Invalid()
        {
            var command = new UpsertSettingsCommand
            {
                Settings = new List<SettingDto> { new SettingDto { Name = "Invalid Name!", Value = DateTime.UtcNow } }
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor("Settings[0].Name");
        }

        [Test]
        public void Should_Not_Have_Error_When_Valid()
        {
            var command = new UpsertSettingsCommand
            {
                Settings = new List<SettingDto> { new SettingDto { Name = "ValidName", Value = DateTime.UtcNow } }
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
