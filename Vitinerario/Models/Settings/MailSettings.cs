namespace Vitinerario.Models.Settings
{
    public class MailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public bool EnableSSL { get; set; }
    }
}
