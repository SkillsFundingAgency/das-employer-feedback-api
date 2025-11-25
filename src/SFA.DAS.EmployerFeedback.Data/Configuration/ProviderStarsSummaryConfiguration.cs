using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderStarsSummaryConfiguration : IEntityTypeConfiguration<ProviderStarsSummary>
    {
        public void Configure(EntityTypeBuilder<ProviderStarsSummary> builder)
        {
            builder.ToTable("ProviderStarsSummary");
            builder.HasKey(e => new { e.Ukprn, e.TimePeriod });
        }
    }
}
