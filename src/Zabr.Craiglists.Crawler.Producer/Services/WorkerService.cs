using Zabr.Craiglists.Crawler.Common.Models.Rabbit;
using Zabr.Craiglists.Crawler.RabbitMq.Interfaces;

namespace Zabr.Craiglists.Crawler.Producer.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly IRabbitMqClientService _rabbitMqClientService;
        private readonly string[] _urls;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(IRabbitMqClientService rabbitMqClientService, ILogger<WorkerService> logger)
        {
            _rabbitMqClientService = rabbitMqClientService;
            _logger = logger;

            _urls = ReadFileWithStartupUrls();
        }

        private static string[] ReadFileWithStartupUrls()
        {
            var path = Path.GetFullPath("InitialPages.txt");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File InitialPages not found");
            }

            return File.ReadAllLines(path);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            while (true)
            {
                foreach (string url in _urls)
                {
                    await EnqueueLink(new QueueItem { Url = url }, stoppingToken);
                }

                await Task.Delay(new TimeSpan(0, 0, 5));
            }

            await Task.CompletedTask;
        }
        

        private async Task EnqueueLink(QueueItem link, CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Sending message: {Url}", link.Url);
            await _rabbitMqClientService.SendMessageAsync(link, stoppingToken);
        }
    }
}
