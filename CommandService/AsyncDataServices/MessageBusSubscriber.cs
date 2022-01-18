using CommandService.EventProcessing;
using RabbitMQ.Client;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    public readonly IConnection _connection;
    private readonly IModel _channel;
    public readonly string _queueName;

    public MessageBusSubscriber(
        IConfiguration configuration,
        IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

          var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue : _queueName, exchange : "trigger", routingKey: "");

        Console.WriteLine("--> Listening on the message bus");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> Connection shutdown");
    }

    public override void Dispose()
    {
        if(_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}