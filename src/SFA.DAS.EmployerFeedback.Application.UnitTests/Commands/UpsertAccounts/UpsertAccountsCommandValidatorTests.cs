using System.Collections.Generic;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpsertAccounts
{
    [TestFixture]
    public class UpsertAccountsCommandValidatorTests
    {
        private UpsertAccountsCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpsertAccountsCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Accounts_Is_Null()
        {
            var command = new UpsertAccountsCommand { Accounts = null };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Accounts);
        }

        [Test]
        public void Should_Have_Error_When_Accounts_Is_Empty()
        {
            var command = new UpsertAccountsCommand { Accounts = new List<AccountUpsertDto>() };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Accounts);
        }

        [Test]
        public void Should_Have_Error_When_AccountId_Is_Invalid()
        {
            var command = new UpsertAccountsCommand
            {
                Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 0, AccountName = "ValidName" } }
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor("Accounts[0].AccountId");
        }

        [Test]
        public void Should_Have_Error_When_AccountName_Is_Invalid()
        {
            var command = new UpsertAccountsCommand
            {
                Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "" } }
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor("Accounts[0].AccountName");
        }

        [Test]
        public void Should_Not_Have_Error_When_Valid()
        {
            var command = new UpsertAccountsCommand
            {
                Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "ValidName" } }
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
