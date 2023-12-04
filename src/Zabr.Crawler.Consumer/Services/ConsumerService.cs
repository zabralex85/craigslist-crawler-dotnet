using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Zabr.Crawler.Common.Extensions;
using Zabr.Crawler.Common.Models.Base;
using Zabr.Crawler.Common.Models.Crawl;
using Zabr.Crawler.Common.Models.Rabbit;
using Zabr.Crawler.Data.Entities;
using Zabr.Crawler.Data.Repositories;
using Zabr.Crawler.RabbitMq.Interfaces;
using Zabr.Crawler.Scrapers.Interfaces;

namespace Zabr.Crawler.Consumer.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IRabbitMqClientService _rabbitMqClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IScrapingService _scrapingService;
        private readonly ILogger<ConsumerService> _logger;
        private readonly EventHandler<BasicDeliverEventArgs> _handler;

        private IModel? _channel;
        private EventingBasicConsumer? _consumer;
        private string? _queue;

        public ConsumerService(
            IRabbitMqClientService rabbitMqClientService,
            IServiceProvider serviceProvider,
            IScrapingService scrapingService,
            ILogger<ConsumerService> logger
        )
        {
            _scrapingService = scrapingService;
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
                await HandleMessageAsync(message, stoppingToken);
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

        private async Task HandleMessageAsync(BaseItem content, CancellationToken token)
        {
            _logger.LogInformation("Received: {Url}", content.Url);

            var resurceType = _scrapingService.RecognizeResource(content.Url);
            var pages = await _scrapingService.ScrapeResourceAsync(resurceType, content.Url, token);

            foreach (var page in pages)
            {
                await SaveInDataBase(new RootPage
                {
                    Url = page.Url,
                    Content = page.Content
                });
            }
        }

        private async Task SaveInDataBase(BasePage rootPage)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var respository = (PageRepository)scope.ServiceProvider.GetRequiredService<IPageRepository>();
                await respository.AddIfNoExistsAsync(new PageEntity(rootPage.Url, DateTime.UtcNow, rootPage.Content));
            }
        }
    }
}

