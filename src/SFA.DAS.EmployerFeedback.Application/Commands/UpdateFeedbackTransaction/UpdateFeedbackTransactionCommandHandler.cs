using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction
{
    public class UpdateFeedbackTransactionCommandHandler : IRequestHandler<UpdateFeedbackTransactionCommand, Unit>
    {
        private readonly IFeedbackTransactionContext _feedbackTransactionContext;
        private readonly ILogger<UpdateFeedbackTransactionCommandHandler> _logger;

        public UpdateFeedbackTransactionCommandHandler(
            IFeedbackTransactionContext feedbackTransactionContext,
            ILogger<UpdateFeedbackTransactionCommandHandler> logger)
        {
            _feedbackTransactionContext = feedbackTransactionContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateFeedbackTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing UpdateFeedbackTransaction for Id: {Id}", request.Id);

                var existingTransaction = await _feedbackTransactionContext.GetByIdAsync(request.Id, cancellationToken);
                
                if (existingTransaction == null)
                {
                    _logger.LogWarning("FeedbackTransaction with Id: {Id} not found", request.Id);
                    throw new InvalidOperationException($"FeedbackTransaction with Id {request.Id} not found");
                }

                existingTransaction.TemplateId = request.TemplateId;
                existingTransaction.SentCount = request.SentCount;
                existingTransaction.SentDate = request.SentDate;

                await _feedbackTransactionContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated FeedbackTransaction for Id: {Id}", request.Id);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UpdateFeedbackTransaction for Id: {Id}", request.Id);
                throw;
            }
        }
    }
}