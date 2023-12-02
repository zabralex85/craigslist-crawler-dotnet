using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Zabr.Craiglists.Crawler.Common.Extensions;
using Zabr.Craiglists.Crawler.Common.Models.Rabbit;
using Zabr.Craiglists.Crawler.Data.Entities;
using Zabr.Craiglists.Crawler.Data.Repositories;
using Zabr.Craiglists.Crawler.RabbitMq.Interfaces;

namespace Zabr.Craiglists.Crawler.Consumer.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IRabbitMqClientService _rabbitMqClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly CraiglistsDirectoryService _craiglistsDirectory;
        private readonly ILogger<ConsumerService> _logger;
        private readonly EventHandler<BasicDeliverEventArgs> _handler;

        private IModel? _channel;
        private EventingBasicConsumer? _consumer;
        private string? _queue;

        public ConsumerService(
            IRabbitMqClientService rabbitMqClientService,
            IServiceProvider serviceProvider,
            CraiglistsDirectoryService craiglistsDirectory,
            ILogger<ConsumerService> logger
        )
        {
            _craiglistsDirectory = craiglistsDirectory;
            _rabbitMqClientService = rabbitMqClientService;
            _serviceProvider = serviceProvider;
            _logger = logger;

            _handler = async (sender, ea) =>
            {
                if (sender != null)
                {
                    await AsyncHanler(ea.Body.ToArray(), ea.DeliveryTag, CancellationToken.None);
                }
            };
        }

        private async Task AsyncHanler(byte[] body, ulong deliveryTag, CancellationToken stoppingToken)
        {
            var message = await body.GetObjectAsync<QueueItem>(stoppingToken);
            if (message != null)
            {
                await HandleMessageAsync(message);
            }
            else
            {
                _logger.LogWarning("Message from queue is null");
            }

            _channel?.BasicAck(deliveryTag: deliveryTag, multiple: false);

            await Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _channel = _rabbitMqClientService.GetChannel();
            if (_channel != null)
            {
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Received += _handler;

                _queue = _rabbitMqClientService.GetDefaultQueue();
                _channel.BasicConsume(_consumer, _queue, autoAck: false);
                
            }

            return Task.CompletedTask;
        }

        private async Task OnReceived(object? sender, BasicDeliverEventArgs ea)
        {
            
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            if (_consumer != null)
            {
                _consumer.Received -= _handler;
            }

            if (_channel != null)
            {
                if (!_channel.IsClosed)
                {
                    _channel.Close();
                }
                _channel.Dispose();
            }

            return Task.CompletedTask;
        }
        
        //private void HandleMessage(QueueItem content)
        //{
        //    _logger.LogInformation("Received: {Url}", content.Url);
            
            
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var respository = (PageRepository)scope.ServiceProvider.GetRequiredService<IPageRepository>();
        //        respository.Add(new PageEntity(content.Url, DateTime.UtcNow, "Content"));
        //    }
        //}

        private async Task HandleMessageAsync(QueueItem content)
        {
            _logger.LogInformation("Received: {Url}", content.Url);

            using (var scope = _serviceProvider.CreateScope())
            {
                var respository = (PageRepository)scope.ServiceProvider.GetRequiredService<IPageRepository>();
                await respository.AddIfNoExistsAsync(new PageEntity(content.Url, DateTime.UtcNow, "Content"));
            }
        }
    }
}

