using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Data.Configuration
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.EmployerFeedbacks)
                   .WithOne()
                   .HasForeignKey(x => x.AccountId);
        }
    }
}
