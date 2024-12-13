using MimeKit;
using MailKit.Net.Smtp;
namespace DongGopTuThien.Services
{
    public class SMTPService: ISMTPService
    {
        private SmtpConfiguration _smtpConfig;

        public SMTPService(SmtpConfiguration smtpConfig)
        {
            _smtpConfig = smtpConfig;
        }
        public async Task SendEmailsAsync(Dictionary<string,string> recipients, string subject, string body)
        {
            // Tạo đối tượng email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hệ thống", _smtpConfig.FromEmail));

            // Thêm người nhận
            foreach (var recipient in recipients)
            {
                message.To.Add(new MailboxAddress(recipient.Value, recipient.Key));
            }

            message.Subject = subject;

            // Nội dung email (HTML)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Kết nối và gửi email qua SMTP
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpConfig.UserName, _smtpConfig.Password);
                    await client.SendAsync(message);
                }
                catch (MailKit.Net.Smtp.SmtpCommandException ex)
                {
                    // Xử lý lỗi khi có vấn đề với lệnh SMTP
                    Console.WriteLine($"SMTP Command Error: {ex.StatusCode} - {ex.Message}");
                    throw;
                }
                catch (MailKit.Net.Smtp.SmtpProtocolException ex)
                {
                    // Xử lý lỗi giao thức SMTP
                    Console.WriteLine($"SMTP Protocol Error: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    // Xử lý các lỗi chung khác
                    Console.WriteLine($"Unexpected Error: {ex.Message}");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }

    public interface ISMTPService
    {
        Task SendEmailsAsync(Dictionary<string, string> recipients, string subject, string body);
    }

    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
    }
}
