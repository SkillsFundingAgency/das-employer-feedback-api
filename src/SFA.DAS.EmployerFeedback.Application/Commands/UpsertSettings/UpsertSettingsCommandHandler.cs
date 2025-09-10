using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertSettingsCommandHandler : IRequestHandler<UpsertSettingsCommand, Unit>
    {
        private readonly ISettingsContext _settingsContext;
        private readonly ILogger<UpsertSettingsCommandHandler> _logger;

        public UpsertSettingsCommandHandler(ISettingsContext settingsContext, ILogger<UpsertSettingsCommandHandler> logger)
        {
            _settingsContext = settingsContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpsertSettingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var req in request.Settings)
                {
                    var existing = await _settingsContext.GetByNameAsync(req.Name, cancellationToken);
                    if (existing != null)
                    {
                        existing.Value = req.Value;
                        _settingsContext.Update(existing);
                    }
                    else
                    {
                        _settingsContext.Add(new Settings { Name = req.Name, Value = req.Value });
                    }
                }
                await _settingsContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                throw;
            }
        }
    }
}
