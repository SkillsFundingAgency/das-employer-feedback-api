using MediatR;
using SFA.DAS.EmployerFeedback.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction
{
    public class UpsertFeedbackTransactionCommand : IRequest<Unit>
    {
        public long AccountId { get; set; }
        public List<ProviderCourseDto> Active { get; set; } = new List<ProviderCourseDto>();
        public List<ProviderCourseDto> Completed { get; set; } = new List<ProviderCourseDto>();
        public List<ProviderCourseDto> NewStart { get; set; } = new List<ProviderCourseDto>();

        public static implicit operator UpsertFeedbackTransactionCommand(UpsertFeedbackTransactionRequest source)
        {
            return new UpsertFeedbackTransactionCommand
            {
                Active = source.Active,
                Completed = source.Completed,
                NewStart = source.NewStart
            };
        }
    }
}