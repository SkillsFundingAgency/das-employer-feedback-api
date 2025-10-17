using MediatR;
using SFA.DAS.EmployerFeedback.Application.Models;
using System;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction
{
    public class UpdateFeedbackTransactionCommand : IRequest<Unit>
    {
        public long Id { get; set; }
        public Guid TemplateId { get; set; }
        public int SentCount { get; set; }
        public DateTime SentDate { get; set; }

        public static implicit operator UpdateFeedbackTransactionCommand(UpdateFeedbackTransactionRequest source)
        {
            return new UpdateFeedbackTransactionCommand
            {
                TemplateId = source.TemplateId,
                SentCount = source.SentCount,
                SentDate = source.SentDate
            };
        }
    }
}