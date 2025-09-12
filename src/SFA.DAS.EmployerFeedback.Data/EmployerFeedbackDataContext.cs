using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerFeedback.Data.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Configuration;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Data
{
    [ExcludeFromCodeCoverage]
    public class EmployerFeedbackDataContext : DbContext,
        IAttributeContext,
        IProviderRatingSummaryContext,
        IEmployerFeedbackContext,
        IEmployerFeedbackResultContext,
        IProviderAttributeContext,
        ISettingsContext,
        IAccountContext 
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly ChainedTokenCredential _chainedTokenCredentialProvider;
        public virtual DbSet<Attributes> Attributes { get; set; }
        public virtual DbSet<ProviderRatingSummary> ProviderRatingSummary { get; set; } = null!;
        public virtual DbSet<Domain.Entities.EmployerFeedback> EmployerFeedback { get; set; }
        public virtual DbSet<EmployerFeedbackResult> EmployerFeedbackResult { get; set; }
        public virtual DbSet<ProviderAttribute> ProviderAttributes { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }

        DbSet<Attributes> IEntityContext<Attributes>.Entities => Attributes;
        DbSet<ProviderRatingSummary> IEntityContext<ProviderRatingSummary>.Entities => ProviderRatingSummary;
        DbSet<Domain.Entities.EmployerFeedback> IEntityContext<Domain.Entities.EmployerFeedback>.Entities => EmployerFeedback;
        DbSet<EmployerFeedbackResult> IEntityContext<EmployerFeedbackResult>.Entities => EmployerFeedbackResult;
        DbSet<ProviderAttribute> IEntityContext<ProviderAttribute>.Entities => ProviderAttributes;
        DbSet<Settings> IEntityContext<Settings>.Entities => Settings;
        DbSet<Account> IEntityContext<Account>.Entities => Accounts;

        public EmployerFeedbackDataContext(IOptions<ApplicationSettings> config,
            DbContextOptions<EmployerFeedbackDataContext> options)
            : base(options)
        {
            _configuration = config.Value;
        }

        public EmployerFeedbackDataContext(IOptions<ApplicationSettings> config,
            DbContextOptions<EmployerFeedbackDataContext> options,
            ChainedTokenCredential chainedTokenCredentialProvider)
            : base(options)
        {
            _configuration = config.Value;
            _chainedTokenCredentialProvider = chainedTokenCredentialProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_chainedTokenCredentialProvider != null)
            {
                var connection = new SqlConnection
                {
                    ConnectionString = _configuration.DbConnectionString,
                    AccessToken = _chainedTokenCredentialProvider
                        .GetTokenAsync(new TokenRequestContext(scopes: [AzureResource]))
                        .GetAwaiter().GetResult().Token
                };

                optionsBuilder.UseSqlServer(connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderRatingSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new EmployerFeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new EmployerFeedbackResultConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new SettingsConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public async Task GenerateFeedbackSummaries()
        {
            var originalTimeout = Database.GetCommandTimeout();

            try
            {
                Database.SetCommandTimeout(120);

                await Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[GenerateProviderAttributeResults]");

                await Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[GenerateProviderRatingResults]");
            }
            finally
            {
                Database.SetCommandTimeout(originalTimeout);
            }
        }
    }
}
