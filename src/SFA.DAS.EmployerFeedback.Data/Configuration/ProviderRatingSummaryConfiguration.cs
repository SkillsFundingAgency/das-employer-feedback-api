using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderRatingSummaryConfiguration : IEntityTypeConfiguration<Domain.Entities.ProviderRatingSummary>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ProviderRatingSummary> builder)
        {
            builder.ToTable("ProviderRatingSummary");
            builder.HasKey(x => new { x.Ukprn, x.Rating, x.TimePeriod });
            builder.Property(x => x.Rating).HasMaxLength(20).IsRequired();
            builder.Property(x => x.TimePeriod).HasMaxLength(50).HasDefaultValue("All").IsRequired();
            builder.Property(x => x.RatingCount).IsRequired();
            builder.Property(x => x.UpdatedOn);
        }
    }
}