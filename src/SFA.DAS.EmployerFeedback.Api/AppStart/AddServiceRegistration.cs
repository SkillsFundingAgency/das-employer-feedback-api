using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerFeedback.Api.TaskQueue;
using SFA.DAS.EmployerFeedback.Application.Behaviours;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using SFA.DAS.EmployerFeedback.Data;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Api.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAttributesQuery).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<IAttributeContext>(s => s.GetRequiredService<EmployerFeedbackDataContext>());
            services.AddScoped<IProviderRatingSummaryContext>(s => s.GetRequiredService<EmployerFeedbackDataContext>());

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}