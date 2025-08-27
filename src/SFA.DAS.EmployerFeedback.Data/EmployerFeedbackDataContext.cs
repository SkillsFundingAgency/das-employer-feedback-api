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

namespace SFA.DAS.EmployerFeedback.Data
{
    [ExcludeFromCodeCoverage]
    public class EmployerFeedbackDataContext : DbContext,
        IAttributeEntityContext
    {
        private const string AzureResource = "https://database.windows.net/";
        private readonly ApplicationSettings _configuration;
        private readonly ChainedTokenCredential _chainedTokenCredentialProvider;
        public virtual DbSet<Attributes> Attributes { get; set; }

        DbSet<Attributes> IEntityContext<Attributes>.Entities => Attributes;

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
            base.OnModelCreating(modelBuilder);
        }
    }
}
