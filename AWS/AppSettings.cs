namespace LogBackend.AWS;
public class AppSettings
{
    public string Secert { get; set; }
    public int RefreshTokenTTL { get; set; }
    public string EmailFrom { get; set; }
    public string SmtpHost { get; set; }
    public int smtpPort { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
}