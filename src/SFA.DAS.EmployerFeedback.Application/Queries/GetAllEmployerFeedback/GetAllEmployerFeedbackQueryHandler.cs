using MediatR;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback
{
    public class GetAllEmployerFeedbackQueryHandler : IRequestHandler<GetAllEmployerFeedbackQuery, GetAllEmployerFeedbackQueryResult>
    {
        private readonly IEmployerFeedbackContext _context;

        public GetAllEmployerFeedbackQueryHandler(IEmployerFeedbackContext context)
        {
            _context = context;
        }

        public async Task<GetAllEmployerFeedbackQueryResult> Handle(GetAllEmployerFeedbackQuery request, CancellationToken cancellationToken)
        {
            var groupedFeedback = await _context.GetAllEmployerFeedbackAsync(cancellationToken);

            return new GetAllEmployerFeedbackQueryResult
            {
                Feedbacks = groupedFeedback
            };
        }
    }
}