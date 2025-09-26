using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderAttributeSummaryConfiguration : IEntityTypeConfiguration<ProviderAttributeSummary>
    {
        public void Configure(EntityTypeBuilder<ProviderAttributeSummary> builder)
        {
            builder.ToTable("ProviderAttributeSummary");
            builder.HasKey(e => new { e.Ukprn, e.AttributeId, e.TimePeriod });
            builder.HasOne(e => e.Attribute)
                .WithMany()
                .HasForeignKey(e => e.AttributeId);
        }
    }
}








