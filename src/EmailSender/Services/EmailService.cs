using EmailSender.Interfaces;
using EmailSender.Models.Dto;

namespace EmailSender.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(EmailDto dto)
    {
        throw new NotImplementedException();
    }
}