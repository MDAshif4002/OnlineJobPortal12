using System.Net.Mail;
using System.Net;

namespace OnlineJobPortal.Services
{
    public class EmailSender
    {
        private readonly SmtpClient _smtpClient;

        public EmailSender()
        {
            _smtpClient = new SmtpClient("mail.digicoders.in")
            {
                Port = 587,
                Credentials = new NetworkCredential("students@digicoders.in", "pZ39wmPIVZ86"),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("students@digicoders.in"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true, // Set this to false if you don't want HTML in the email
            };

            mailMessage.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mailMessage);
        }

    }
}
