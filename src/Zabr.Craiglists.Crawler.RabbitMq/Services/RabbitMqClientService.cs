using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Zabr.Craiglists.Crawler.RabbitMq.Interfaces;
using Zabr.Craiglists.Crawler.RabbitMq.Options;

namespace Zabr.Craiglists.Crawler.RabbitMq.Services
{
    public class RabbitMqClientService : IRabbitMqClientService
    {
        public IModel GetChannel() => _channel;
        public string GetDefaultQueue() => _queue;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string _queue;
        private readonly string _exchange;
        private readonly bool _queueIsDurable;
        private readonly bool _queueIsExclusive;
        private readonly bool _queueIsAutoDelete;

        public RabbitMqClientService(IOptions<RabbitMqOptions> options)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Host,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                RequestedConnectionTimeout = new TimeSpan(0,0,10)
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

            _channel = _connection.CreateModel();

            _exchange = options.Value.DefaultExchange;
            _queue = options.Value.DefaultQueue;
            _queueIsDurable = options.Value.DefaultQueueIsDurable;
            _queueIsExclusive = options.Value.DefaultQueueIsExclusive;
            _queueIsAutoDelete = options.Value.DefaultQueueIsAutoDelete;

            // Exchange declaration
            _channel.ExchangeDeclare(exchange: _exchange,
                type: ExchangeType.Direct, // Can be Direct, Fanout, Topic, Headers
                durable: true,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(_queue, durable: _queueIsDurable, exclusive: _queueIsExclusive, autoDelete: _queueIsAutoDelete, arguments: null);
        }

        public void SendMessage(object obj, bool isPersistent = true)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = isPersistent; // Makes the message durable

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

            _channel.BasicPublish(exchange: _exchange,
                routingKey: _queue,
                basicProperties: properties,
                body: body);
        }

        public async Task SendMessageAsync(object obj, CancellationToken stoppingToken, bool isPersistent = true)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = isPersistent; // Makes the message durable
            
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

            await Task.Run(() =>
            {
                _channel.BasicPublish(exchange: _exchange,
                    routingKey: _queue,
                    basicProperties: properties,
                    body: body);
            }, stoppingToken);
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
