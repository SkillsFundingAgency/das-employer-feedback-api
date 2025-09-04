using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerFeedback.Api.TaskQueue;
using SFA.DAS.EmployerFeedback.Application.Behaviours;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
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

            services.AddValidatorsFromAssemblyContaining<SubmitEmployerFeedbackCommand>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddScoped<IEmployerFeedbackContext>(sp => sp.GetRequiredService<EmployerFeedbackDataContext>());
            services.AddScoped<IEmployerFeedbackResultContext>(sp => sp.GetRequiredService<EmployerFeedbackDataContext>());
            services.AddScoped<IProviderAttributeContext>(sp => sp.GetRequiredService<EmployerFeedbackDataContext>());
            services.AddScoped<IAttributeContext>(sp => sp.GetRequiredService<EmployerFeedbackDataContext>());
            services.AddScoped<IProviderRatingSummaryContext>(sp => sp.GetRequiredService<EmployerFeedbackDataContext>());

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}