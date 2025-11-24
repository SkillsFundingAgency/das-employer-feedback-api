using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProviderAttributeEntity = SFA.DAS.EmployerFeedback.Domain.Entities.ProviderAttribute;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderAttributeConfiguration : IEntityTypeConfiguration<ProviderAttributeEntity>
    {
        public void Configure(EntityTypeBuilder<ProviderAttributeEntity> entity)
        {
            entity.ToTable("ProviderAttributes");
            entity.HasKey(e => new { e.EmployerFeedbackResultId, e.AttributeId });
            entity.HasOne(e => e.EmployerFeedbackResult)
                .WithMany(r => r.ProviderAttributes)
                .HasForeignKey(e => e.EmployerFeedbackResultId);
            entity.HasOne(e => e.Attribute) 
                .WithMany()
                .HasForeignKey(e => e.AttributeId);
        }
    }
}