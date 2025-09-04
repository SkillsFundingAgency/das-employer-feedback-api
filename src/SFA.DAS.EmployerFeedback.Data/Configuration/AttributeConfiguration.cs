using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AttributeConfiguration : IEntityTypeConfiguration<Attributes>
    {
        public void Configure(EntityTypeBuilder<Attributes> builder)
        {
            builder.ToTable("Attributes");
            builder.HasKey(e => e.AttributeId);
            builder.Property(e => e.AttributeName).HasMaxLength(250).IsRequired();
            builder.HasIndex(e => new { e.AttributeId, e.AttributeName });
        }
    }
}