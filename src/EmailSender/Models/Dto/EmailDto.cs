using System.ComponentModel.DataAnnotations;

namespace EmailSender.Models.Dto;

public class EmailDto
{
    [Required] public string From { get; set; }
    [Required] public string To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}