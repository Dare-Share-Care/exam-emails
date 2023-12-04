using EmailSender.Models.Dto;

namespace EmailSender.Interfaces;

public interface IEmailService
{ 
    Task SendEmailAsync(EmailDto dto);
}