using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    public class EmployerFeedbackResultConfiguration : IEntityTypeConfiguration<EmployerFeedbackResult>
    {
        public void Configure(EntityTypeBuilder<EmployerFeedbackResult> builder)
        {
            builder.ToTable("EmployerFeedbackResult");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FeedbackId).IsRequired();
            builder.Property(x => x.DateTimeCompleted).IsRequired();
            builder.Property(x => x.ProviderRating).IsRequired().HasMaxLength(20).IsUnicode(false);

            builder.HasOne(x => x.EmployerFeedbackTarget)
                   .WithMany()
                   .HasForeignKey(x => x.FeedbackId)
                   .HasConstraintName("FK_EmployerFeedbackResult_EmployerFeedback_FeedbackId");
        }
    }
}
