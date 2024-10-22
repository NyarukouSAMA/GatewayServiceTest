namespace ExtService.Correspondence.Models.Options
{
    public class EmailExchangeOptions
    {
        public static string EmailExchangeOptionsSection = "EmailExchange";
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; } = 587; // Default SMTP port for TLS
        public bool UseSsl { get; set; } = true; // Whether to use SSL/TLS
        public bool UseAuthentication { get; set; } = true;
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmailAddress { get; set; }
        public string FromDisplayName { get; set; }
        public bool SaveEmailCopy { get; set; } = false;
        public string SaveCopyEmailAddress { get; set; }
    }
}
