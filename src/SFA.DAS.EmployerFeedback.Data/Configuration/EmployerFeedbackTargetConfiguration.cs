using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    public class EmployerFeedbackTargetConfiguration : IEntityTypeConfiguration<EmployerFeedbackTarget>
    {
        public void Configure(EntityTypeBuilder<EmployerFeedbackTarget> builder)
        {
            builder.ToTable("EmployerFeedback");
            builder.HasKey(x => x.FeedbackId);
            builder.Property(x => x.FeedbackId).ValueGeneratedOnAdd();
            builder.Property(x => x.UserRef).IsRequired();
            builder.Property(x => x.Ukprn).IsRequired();
            builder.Property(x => x.AccountId).IsRequired();
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(false);

            builder.HasOne(x => x.Account)
                   .WithMany(x => x.EmployerFeedbackTargets);

            builder.HasMany(x => x.EmployerFeedbackResults)
                .WithOne(x => x.EmployerFeedbackTarget);
        }
    }
}
