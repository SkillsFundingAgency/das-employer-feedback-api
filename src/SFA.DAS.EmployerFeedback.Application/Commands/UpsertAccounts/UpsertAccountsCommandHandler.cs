using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts
{
    public class UpsertAccountsCommandHandler : IRequestHandler<UpsertAccountsCommand, Unit>
    {
        private readonly IAccountContext _accountContext;
        private readonly ILogger<UpsertAccountsCommandHandler> _logger;

        public UpsertAccountsCommandHandler(IAccountContext accountContext, ILogger<UpsertAccountsCommandHandler> logger)
        {
            _accountContext = accountContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpsertAccountsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting upsert for accounts");

            try
            {
                var accountIds = request.Accounts.Select(a => a.AccountId).ToList();

                var existingAccounts = await _accountContext.GetAccountsByIdsAsync(accountIds, cancellationToken);

                var existingAccountsDict = existingAccounts.ToDictionary(a => a.Id);

                foreach (var dto in request.Accounts)
                {
                    if (existingAccountsDict.TryGetValue(dto.AccountId, out var existing))
                    {
                        existing.AccountName = dto.AccountName;
                        existing.CheckedOn = null;
                        _accountContext.Update(existing);
                    }
                    else
                    {
                        var account = new Account
                        {
                            Id = dto.AccountId,
                            AccountName = dto.AccountName,
                            CheckedOn = null
                        };
                        _accountContext.Add(account);
                    }
                }

                await _accountContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully upserted accounts");

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting");
                throw;
            }
        }
    }
}
