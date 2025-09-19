using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;

namespace SFA.DAS.EmployerFeedback.Application.Models
{
    public class UpsertAccountsRequest
    {
        public List<AccountUpsertDto> Accounts { get; set; }
    }
}
