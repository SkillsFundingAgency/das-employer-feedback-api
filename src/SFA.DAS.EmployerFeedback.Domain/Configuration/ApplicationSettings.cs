namespace SFA.DAS.EmployerFeedback.Domain.Configuration
{
    public class ApplicationSettings
    {
        public AzureActiveDirectoryApiConfiguration AzureAd { get; set; }

        public string DbConnectionString { get; set; }
        
        public int BatchDays { get; set; } = 30;
    }
}
