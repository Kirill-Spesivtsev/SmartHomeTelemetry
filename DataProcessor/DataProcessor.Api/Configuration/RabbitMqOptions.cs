namespace DataProcessor.Api.Configuration;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public string Host { get; set; } = "rabbitmq";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}
