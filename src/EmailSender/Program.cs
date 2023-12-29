using System.Text.Json;
using Confluent.Kafka;
using EmailSender.Models.Dto;
using EmailSender.Services;

namespace EmailSender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Load .env file
            DotNetEnv.Env.Load();

            // Kafka consumer configuration
            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:9093",
                GroupId = "email_group_group",
                AutoOffsetReset = AutoOffsetReset.Latest
            };
            using var c = new ConsumerBuilder<Ignore, string>(config).Build();
            c.Subscribe("mtogo-send-email");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };
            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume(cts.Token);
                        // Process the received message
                        var emailDetails = JsonSerializer.Deserialize<EmailDto>(cr.Value);

                        var emailDto = new EmailDto
                        {
                            To = emailDetails.To,
                            Subject = emailDetails.Subject,
                            Body = emailDetails.Body
                        };
                        
                        var emailService = new EmailService();
                        
                        const string mail = "8ffae84449bb32";
                        const string trap = "0bd2bb5c971229";
                        
                        //Send email
                        await emailService.SendEmailAsync(emailDto, mail, trap);
                        

                        Console.WriteLine($"Sending email.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                        // Log the full exception details for debugging purposes
                        Console.WriteLine($"Exception details: {e}");
                    }
                    catch (OperationCanceledException)
                    {
                        // Log cancellation of the operation (optional)
                        Console.WriteLine("Operation was cancelled.");
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        c.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex}");
            }
        }
    }
}