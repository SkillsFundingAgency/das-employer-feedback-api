namespace SFA.DAS.EmployerFeedback.Domain.Configuration
{
    public class ApplicationSettings
    {
        public AzureActiveDirectoryApiConfiguration AzureAd { get; set; }

        public string DbConnectionString { get; set; }
        
        public int EmailNudgeCheckDays { get; set; } = 30;
    }
}
