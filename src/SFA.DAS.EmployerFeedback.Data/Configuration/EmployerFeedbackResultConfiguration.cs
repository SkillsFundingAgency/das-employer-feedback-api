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
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.EmployerFeedback)
                .WithMany(f => f.FeedbackResults)
                .HasForeignKey(e => e.FeedbackId);
        }
    }
}
