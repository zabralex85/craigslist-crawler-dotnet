using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            //_channel = _rabbitMqClientService.GetChannel();
            //_channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            //_consumer = new EventingBasicConsumer(_channel);

            //_consumer.Received += OnReceived;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            //_consumer.Received -= OnReceived;
            //_consumer.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var channel = _rabbitMqClientService.GetChannel();
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<QueueItem>(Encoding.UTF8.GetString(body), JsonSerializerOptions.Default);

                if (message != null)
                {
                    HandleMessage(message);
                }
                else
                {
                    _logger.LogWarning("Message from queue is null");
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            var queue = _rabbitMqClientService.GetDefaultQueue();
            channel.BasicConsume(consumer, queue, autoAck: false);

            await Task.CompletedTask;
        }

        private void HandleMessage(QueueItem content)
        {
            _logger.LogInformation("Received: {Url}", content.Url);

            using (var scope = _serviceProvider.CreateScope())
            {
                var respository = (PageRepository)scope.ServiceProvider.GetRequiredService<IPageRepository>();
                respository.Add(new PageEntity(content.Url, DateTime.UtcNow, "Content"));
            }
        }

        private async Task HandleMessageAsync(QueueItem content)
        {
            _logger.LogInformation("Received: {Url}", content.Url);

            using (var scope = _serviceProvider.CreateScope())
            {
                var respository = (PageRepository)scope.ServiceProvider.GetRequiredService<IPageRepository>();
                await respository.AddAsync(new PageEntity(content.Url, DateTime.UtcNow, "Content"));
            }
        }
    }
}

