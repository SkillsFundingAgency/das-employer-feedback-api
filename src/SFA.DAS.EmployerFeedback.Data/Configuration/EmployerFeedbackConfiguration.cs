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
            builder.HasIndex(e => new { e.UserRef, e.Ukprn, e.AccountId }).IsUnique(true);
            builder.Property(e => e.UserRef).IsRequired();
            builder.Property(e => e.Ukprn).IsRequired();
            builder.Property(e => e.AccountId).IsRequired();
            builder.Property(e => e.IsActive).IsRequired();
        }
    }
}
