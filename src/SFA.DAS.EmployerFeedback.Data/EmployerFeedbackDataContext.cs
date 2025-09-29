﻿using Azure.Core;
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
        IAccountContext,
        IAttributeContext,
        IEmployerFeedbackContext,
        IEmployerFeedbackResultContext,
        IProviderAttributeContext,
        IProviderRatingSummaryContext,
        IProviderStarsSummaryContext,
        IProviderAttributeSummaryContext,
        ISettingsContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly ChainedTokenCredential _chainedTokenCredentialProvider;

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<Domain.Entities.EmployerFeedback> EmployerFeedbacks { get; set; }
        public virtual DbSet<EmployerFeedbackResult> EmployerFeedbackResults { get; set; } = null!;

        public virtual DbSet<ProviderAttribute> ProviderAttributes { get; set; }
        public virtual DbSet<ProviderRatingSummary> ProviderRatingSummaries { get; set; } = null!;

        public virtual DbSet<ProviderStarsSummary> ProviderStarsSummaries { get; set; }
        public virtual DbSet<ProviderAttributeSummary> ProviderAttributeSummaries { get; set; }

        public virtual DbSet<Settings> Settings { get; set; }

        DbSet<Account> IEntityContext<Account>.Entities => Accounts;
        DbSet<Attribute> IEntityContext<Attribute>.Entities => Attributes;
        DbSet<Domain.Entities.EmployerFeedback> IEntityContext<Domain.Entities.EmployerFeedback>.Entities => EmployerFeedbacks;
        DbSet<EmployerFeedbackResult> IEntityContext<EmployerFeedbackResult>.Entities => EmployerFeedbackResults;
        DbSet<ProviderRatingSummary> IEntityContext<ProviderRatingSummary>.Entities => ProviderRatingSummaries;
        DbSet<ProviderAttribute> IEntityContext<ProviderAttribute>.Entities => ProviderAttributes;
        DbSet<ProviderStarsSummary> IEntityContext<ProviderStarsSummary>.Entities => ProviderStarsSummaries;
        DbSet<ProviderAttributeSummary> IEntityContext<ProviderAttributeSummary>.Entities => ProviderAttributeSummaries;
        DbSet<Settings> IEntityContext<Settings>.Entities => Settings;


        public EmployerFeedbackDataContext(DbContextOptions<EmployerFeedbackDataContext> options)
            : base(options)
        {
        }

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
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AttributeConfiguration());
            modelBuilder.ApplyConfiguration(new EmployerFeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new EmployerFeedbackResultConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderRatingSummaryConfiguration());

            modelBuilder.ApplyConfiguration(new ProviderAttributeSummaryConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderStarsSummaryConfiguration());

            modelBuilder.ApplyConfiguration(new SettingsConfiguration());

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
