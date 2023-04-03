using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration configuration;
    private readonly IEventProcessor eventProcessor;
    private IConnection connection;
    private IModel channel;
    private string queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        this.configuration = configuration;
        this.eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (ModuleHandle, ea) => {
            Console.WriteLine("QR501: Event recieved");

            var body = ea.Body;

            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            eventProcessor.ProcessEvent(notificationMessage);
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private void InitializeRabbitMQ() {
        var factory = new ConnectionFactory {
            HostName = configuration["RabbitMQHost"],
            Port = Int32.Parse(configuration["RabbitMQPort"])
        };

        connection = factory.CreateConnection();

        channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

        queueName = channel.QueueDeclare().QueueName;

        channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

        Console.WriteLine("QR501: Listening to the MEssage Bus...");

        connection.ConnectionShutdown += OnRabbitMQConnectionShutdown;
    }

    private void OnRabbitMQConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("QR501: RabbitMQ Connection Shutdown");
    }

    public override void Dispose()
    {
        if(channel.IsOpen) {
            channel.Close();
            connection.Close();
        }

        base.Dispose();
    }
}