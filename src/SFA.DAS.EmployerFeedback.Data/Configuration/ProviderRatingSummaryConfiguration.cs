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
            builder.HasKey(x => new { x.Ukprn, x.Rating,x.TimePeriod });
        }
    }
}