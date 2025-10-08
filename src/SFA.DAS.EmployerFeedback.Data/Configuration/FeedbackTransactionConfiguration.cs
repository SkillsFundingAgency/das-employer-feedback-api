using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class FeedbackTransactionConfiguration : IEntityTypeConfiguration<FeedbackTransaction>
    {
        public void Configure(EntityTypeBuilder<FeedbackTransaction> builder)
        {
            builder.ToTable("FeedbackTransaction");
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Account)
                   .WithMany(a => a.FeedbackTransactions)
                   .HasForeignKey(e => e.AccountId);
        }
    }
}