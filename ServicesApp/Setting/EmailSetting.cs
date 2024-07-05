namespace ServicesApp.EmailSetting
{
    public class EmailSetting
    {
        public string from { get; set; } = string.Empty;

        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
