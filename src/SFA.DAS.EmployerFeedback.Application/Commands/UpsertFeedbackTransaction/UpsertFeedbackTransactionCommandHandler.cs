using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction
{
    public class UpsertFeedbackTransactionCommandHandler : IRequestHandler<UpsertFeedbackTransactionCommand, Unit>
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly IAccountContext _accountContext;
        private readonly ApplicationSettings _applicationSettings;
        private readonly ILogger<UpsertFeedbackTransactionCommandHandler> _logger;
        private readonly IDateTimeHelper _dateTimeHelper;

        public UpsertFeedbackTransactionCommandHandler(
            IFeedbackTransactionContext feedbackTransactionContext,
            IAccountContext accountContext,
            ApplicationSettings applicationSettings,
            ILogger<UpsertFeedbackTransactionCommandHandler> logger,
            IDateTimeHelper dateTimeHelper)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _accountContext = accountContext;
            _applicationSettings = applicationSettings;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<Unit> Handle(UpsertFeedbackTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing UpsertFeedbackTransaction for AccountId: {AccountId}", request.AccountId);

                var sendAfter = CalculateSendAfter(request);

                if (sendAfter.HasValue)
                {
                    await ProcessFeedbackTransaction(request.AccountId, sendAfter.Value, cancellationToken);
                }

                await UpdateAccountCheckedOn(request.AccountId, cancellationToken);
                await _feedbackTransactionContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully processed UpsertFeedbackTransaction for AccountId: {AccountId}", request.AccountId);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UpsertFeedbackTransaction for AccountId: {AccountId}", request.AccountId);
                throw;
            }
        }

        private DateTime? CalculateSendAfter(UpsertFeedbackTransactionCommand request)
        {
            var currentDate = _dateTimeHelper.Now;
            var batchDays = _applicationSettings.BatchDays;

            if (request.NewStart?.Any() == true) return currentDate;
            if (request.Completed?.Any() == true) return currentDate.AddDays(batchDays);
            if (request.Active?.Any() == true) return currentDate.AddDays(2 * batchDays);

            return null;
        }

        private async Task ProcessFeedbackTransaction(long accountId, DateTime sendAfter, CancellationToken cancellationToken)
        {
            var existingTransaction = await _feedbackTransactionContext.GetMostRecentByAccountIdAsync(accountId, cancellationToken);

            if (existingTransaction == null)
            {
                CreateFeedbackTransaction(accountId, sendAfter);
            }
            else if (existingTransaction.SentDate == null)
            {
                if (sendAfter < existingTransaction.SendAfter)
                {
                    existingTransaction.SendAfter = sendAfter;
                }
            }
            else
            {
                CreateFeedbackTransaction(accountId, sendAfter);
            }
        }

        private void CreateFeedbackTransaction(long accountId, DateTime sendAfter)
        {
            var feedbackTransaction = new FeedbackTransaction
            {
                AccountId = accountId,
                SendAfter = sendAfter,
                TemplateName = _applicationSettings.DefaultEmployerFeedbackRequestTemplateName,
                TemplateId = null,
                SentCount = null,
                SentDate = null,
                CreatedOn = _dateTimeHelper.Now
            };

            _feedbackTransactionContext.Add(feedbackTransaction);
        }

        private async Task UpdateAccountCheckedOn(long accountId, CancellationToken cancellationToken)
        {
            var account = await _accountContext.GetByIdAsync(accountId, cancellationToken);
            if (account != null)
            {
                account.CheckedOn = _dateTimeHelper.Now;
            }
        }
    }
}