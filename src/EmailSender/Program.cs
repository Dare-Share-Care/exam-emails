using Confluent.Kafka;
using System.Text.Json;
using EmailSender.Models.Dto;
using EmailSender.Services;
using DotNetEnv;

namespace EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load .env file
            DotNetEnv.Env.Load();
            
            var cts = new CancellationTokenSource();

            // Kafka consumer configuration
            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:9093",
                GroupId = "email_group_id",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("mtogo-send-email");

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cts.Token);

                        // Process the received message
                        var emailDetails = JsonSerializer.Deserialize<EmailDto>(consumeResult.Message.Value);

                        var emailDto = new EmailDto
                        {
                            From = emailDetails.From,
                            To = emailDetails.To,
                            Subject = emailDetails.Subject,
                            Body = emailDetails.Body
                        };


                        var emailService = new EmailService();

                        //Send email
                        
                        //Get mailtrap credentials
                        var username = Environment.GetEnvironmentVariable("USERNAME")!;
                        var password = Environment.GetEnvironmentVariable("PASSWORD")!;

                        
                        // emailService.SendEmailAsync(emailDto).GetAwaiter().GetResult();
                        Console.WriteLine("Email sent successfully!");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }
}