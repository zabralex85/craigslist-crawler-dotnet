//using HtmlAgilityPack;
//using Zabr.Craiglists.Crawler.Common.Models.Base;
//using Zabr.Craiglists.Crawler.RabbitMq.Services;

//namespace Zabr.Craiglists.Crawler.Consumer.Services
//{
//    public class CrawlerService : BackgroundService
//    {
//        private readonly RabbitMqClientService _rabbitMqClientService;
//        private readonly CraiglistsDirectoryService _craiglistsDirectory;
//        private readonly string[] _urls;

//        public CrawlerService(RabbitMqClientService rabbitMqClientService, CraiglistsDirectoryService craiglistsDirectory)
//        {
//            _rabbitMqClientService = rabbitMqClientService;
//            _craiglistsDirectory = craiglistsDirectory;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();


//            foreach (string initialPage in _urls)
//            {
//                var pages = await _craiglistsDirectory.GetPagesAsync(initialPage, stoppingToken);
//                if (pages == null) continue;

//                foreach (var page in pages)
//                {
//                    ProcessPage(page, stoppingToken);
//                }
//            }

//            await Task.CompletedTask;
//        }

//        private void ProcessPage(BasePage pageContent, CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();

//            var links = ExtractLinks(pageContent, stoppingToken);

//            foreach (var link in links)
//            {
//                EnqueueLink(link, stoppingToken);
//            }
//        }

//        private static IEnumerable<string> ExtractLinks(BasePage page, CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();

//            var doc = new HtmlDocument();
//            doc.LoadHtml(page.Content);

//            var links = doc.DocumentNode.SelectNodes("//a[@href]")
//                .Select(node => node.GetAttributeValue("href", string.Empty))
//                .Where(href => !string.IsNullOrEmpty(href));

//            return links;
//        }


//        private void EnqueueLink(string link, CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();

//            _rabbitMqClientService.SendMessage(link);
//        }
//    }
//}
