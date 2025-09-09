using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EmployerFeedbackEntity = SFA.DAS.EmployerFeedback.Domain.Entities.EmployerFeedback;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EmployerFeedbackConfiguration : IEntityTypeConfiguration<EmployerFeedbackEntity>
    {
        public void Configure(EntityTypeBuilder<EmployerFeedbackEntity> builder)
        {
            builder.ToTable("EmployerFeedback");
            builder.HasKey(e => e.FeedbackId);
        }
    }
}
