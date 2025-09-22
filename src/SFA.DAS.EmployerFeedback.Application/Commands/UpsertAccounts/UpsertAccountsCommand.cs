using MediatR;
using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Application.Models;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts
{
    public class UpsertAccountsCommand : IRequest<Unit>
    {
        public List<AccountUpsertDto> Accounts { get; set; }

        public static implicit operator UpsertAccountsCommand(UpsertAccountsRequest source)
        {
            return new UpsertAccountsCommand
            {
                Accounts = source.Accounts
            };
        }
    }
    public class AccountUpsertDto
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; }
    }
}


