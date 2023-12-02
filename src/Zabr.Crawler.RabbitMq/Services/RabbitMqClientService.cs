using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Zabr.Crawler.Common.Extensions;
using Zabr.Crawler.RabbitMq.Interfaces;
using Zabr.Crawler.RabbitMq.Options;

namespace Zabr.Crawler.RabbitMq.Services
{
    public class RabbitMqClientService : IRabbitMqClientService
    {
        public IModel? GetChannel() => _channel;
        public string? GetDefaultQueue() => _queue;

        private readonly ILogger<RabbitMqClientService> _logger;
        private IConnection? _connection;
        private IModel? _channel;
        
        private readonly string? _queue;
        private readonly string? _exchange;
        private readonly bool _queueIsDurable;
        private readonly bool _queueIsExclusive;
        private readonly bool _queueIsAutoDelete;
        private readonly string? _routingKey;
        

        public RabbitMqClientService(IOptions<RabbitMqOptions> options, ILogger<RabbitMqClientService> logger)
        {
            _logger = logger;
            _exchange = options.Value.DefaultExchange;
            _queue = options.Value.DefaultQueue;
            _routingKey = options.Value.DefaultRoutingKey;
            _queueIsDurable = options.Value.DefaultQueueIsDurable;
            _queueIsExclusive = options.Value.DefaultQueueIsExclusive;
            _queueIsAutoDelete = options.Value.DefaultQueueIsAutoDelete;
            
            ConnectAndEnsureCreated(options);
        }

        private void ConnectAndEnsureCreated(IOptions<RabbitMqOptions> options)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Host,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                RequestedConnectionTimeout = new TimeSpan(0, 0, 10)
            };

            int counter = 0;

            resetConnection:

            try
            {
                _connection = factory.CreateConnection();
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
            {
                Thread.Sleep(new TimeSpan(0, 0, 5));
                counter++;

                if (counter <= 5)
                {
                    goto resetConnection;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Can't establish connection to RabbitMq");
            }

            if (_connection is null)
            {
                throw new Exception("Connection to RabbitMq is null");
            }

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queue,
                durable: _queueIsDurable,
                exclusive: _queueIsExclusive,
                autoDelete: _queueIsAutoDelete,
                arguments: null);

            _channel.ExchangeDeclare(exchange: _exchange,
                type: ExchangeType.Direct, // Can be Direct, Fanout, Topic, Headers
                durable: true,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(_queue, _exchange, _routingKey);
        }

        public void SendMessage(object obj, bool isPersistent = true)
        {
            if (_channel != null)
            {
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = isPersistent; // Makes the message durable

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

                _channel.BasicPublish(
                    exchange: _exchange,
                    routingKey: _routingKey,
                    basicProperties: properties,
                    body: body);
            }
        }

        public async Task SendMessageAsync(object obj, CancellationToken stoppingToken, bool isPersistent = false)
        {
            if (_channel != null)
            {
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = isPersistent;
                properties.MessageId = Guid.NewGuid().ToString();

                var json = await obj.GetJsonAsync(stoppingToken);
                var body = Encoding.UTF8.GetBytes(json);

                if (!_channel.IsOpen)
                {
                    
                }

                _channel.BasicPublish(
                    exchange: _exchange,
                    routingKey: _routingKey,
                    basicProperties: properties,
                    body: body,
                    mandatory: false);

                await Task.CompletedTask;
                //await Task.Run(() =>
                //{
                    
                //}, stoppingToken);
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
