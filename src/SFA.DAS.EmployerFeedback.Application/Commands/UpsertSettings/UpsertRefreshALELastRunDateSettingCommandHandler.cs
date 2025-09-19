using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings
{
    public class UpsertRefreshALELastRunDateSettingCommandHandler : IRequestHandler<UpsertRefreshALELastRunDateSettingCommand, Unit>
    {
        private readonly ISettingsContext _settingsContext;
        private readonly ILogger<UpsertRefreshALELastRunDateSettingCommandHandler> _logger;

        public UpsertRefreshALELastRunDateSettingCommandHandler(ISettingsContext settingsContext, ILogger<UpsertRefreshALELastRunDateSettingCommandHandler> logger)
        {
            _settingsContext = settingsContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpsertRefreshALELastRunDateSettingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var name = SettingType.RefreshALELastRunDate.ToString();
                var existing = await _settingsContext.GetByNameAsync(name, cancellationToken);
                if (existing != null)
                {
                    existing.Value = request.Value;
                    _settingsContext.Update(existing);
                }
                else
                {
                    _settingsContext.Add(new Settings { Name = name, Value = request.Value });
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
