using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EmployerFeedbackResultEntity = SFA.DAS.EmployerFeedback.Domain.Entities.EmployerFeedbackResult;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EmployerFeedbackResultConfiguration : IEntityTypeConfiguration<EmployerFeedbackResultEntity>
    {
        public void Configure(EntityTypeBuilder<EmployerFeedbackResultEntity> entity)
        {
            entity.ToTable("EmployerFeedbackResult");
            entity.HasKey(e => e.Id).IsClustered(false);
            entity.Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
            entity.Property(e => e.FeedbackId).IsRequired();
            entity.Property(e => e.DateTimeCompleted).IsRequired();
            entity.Property(e => e.ProviderRating).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FeedbackSource).HasDefaultValue(1);
            entity.HasOne(e => e.EmployerFeedback)
                .WithMany(f => f.FeedbackResults)
                .HasForeignKey(e => e.FeedbackId);
            entity.HasIndex(e => new { e.Id, e.FeedbackId }).IncludeProperties(e => new { e.DateTimeCompleted, e.ProviderRating });
            entity.HasIndex(e => new { e.FeedbackId, e.DateTimeCompleted }).IsUnique();
        }
    }
}
