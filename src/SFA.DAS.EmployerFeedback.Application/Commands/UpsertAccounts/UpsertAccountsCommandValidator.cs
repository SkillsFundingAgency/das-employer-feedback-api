using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts
{
    public class UpsertAccountsCommandValidator : AbstractValidator<UpsertAccountsCommand>
    {
        public UpsertAccountsCommandValidator()
        {
            RuleFor(x => x.Accounts)
                .NotNull().WithMessage("Accounts list is required.")
                .Must(x => x != null && x.Count > 0).WithMessage("Accounts list cannot be empty.");
            RuleForEach(x => x.Accounts).SetValidator(new AccountUpsertDtoValidator());
        }
    }

    public class AccountUpsertDtoValidator : AbstractValidator<AccountUpsertDto>
    {
        public AccountUpsertDtoValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0);

            RuleFor(x => x.AccountName)
                .NotEmpty();
        }
    }
}
