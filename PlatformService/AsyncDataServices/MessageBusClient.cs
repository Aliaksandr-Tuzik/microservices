using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IDisposable
{
    private readonly IConfiguration configuration;
    private readonly IConnection connection;
    private readonly IModel channel;

    public MessageBusClient(IConfiguration configuration)
    {
        this.configuration = configuration;

        var factory = new ConnectionFactory() {
            HostName = configuration["RabbitMQHost"],
            Port = Int32.Parse(configuration["RabbitMQPort"])
        };

        try
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            connection.ConnectionShutdown += OnRabbitMQConnectionShutdown;

            Console.WriteLine("QR501: Connected to Message Bus");
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"QR501: Could not connect to the Message Bus: {ex}");
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if(connection.IsOpen) {
            Console.WriteLine("QR501: RabbitMQ connection open. Sending message...");
            SendMessage(message);
        } else {
            Console.WriteLine("QR501: RabbitMQ connection closed. Not sending message");
        }
    }

    public void Dispose()
    {
        if(channel.IsOpen) {
            channel.Close();
            connection.Close();
        }
    }

    private void SendMessage(string message) {
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "trigger", 
            routingKey: "", 
            basicProperties: null,
            body: body);

        Console.WriteLine("QR501: RabbitMQ message sent");
    }

    private void OnRabbitMQConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("QR501: RabbitMQ Connection Shutdown");
    }
}