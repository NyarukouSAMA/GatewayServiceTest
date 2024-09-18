namespace ExtService.Correspondence.Models.Options
{
    public class EmailExchangeOptions
    {
        public static string EmailExchangeOptionsSection = "EmailExchange";
        public string ExchangeUri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public int ExchangeVersion { get; set; }
        public bool SaveEmailCopy { get; set; } = false;
        public string SaveCopyFolderWellKnownName { get; set; }
    }
}
