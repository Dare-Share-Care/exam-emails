using EmailSender.Interfaces;
using EmailSender.Models.Dto;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailSender.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(EmailDto dto, string mail, string trap)
        {
            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, useSsl: false);
                await client.AuthenticateAsync(mail, trap);

                var bodyBuilder = new BodyBuilder()
                {
                    TextBody = dto.Body
                };

                var email = new MimeMessage()
                {
                    From = { new MailboxAddress("From", "mtogobot@gmail.com") },
                    To = { new MailboxAddress("To", dto.To) },
                    Subject = dto.Subject,
                    Body = bodyBuilder.ToMessageBody()
                };

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw; // Re-throw the exception to handle it at a higher level
            }
        }
    }
}