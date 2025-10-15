namespace SFA.DAS.EmployerFeedback.Domain.Configuration
{
    public class ApplicationSettings
    {
        public AzureActiveDirectoryApiConfiguration AzureAd { get; set; }

        public string DbConnectionString { get; set; }

        public string DefaultEmployerFeedbackRequestTemplateName { get; set; }

        public int EmailNudgeCheckDays { get; set; } = 30;
        public int EmailNudgeSendAfterDays { get; set; } = 31;
    }
}
