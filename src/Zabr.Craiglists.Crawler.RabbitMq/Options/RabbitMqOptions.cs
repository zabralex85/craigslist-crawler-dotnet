namespace Zabr.Craiglists.Crawler.RabbitMq.Options;

public class RabbitMqOptions
{
    public string Host { get; set; } // = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string DefaultExchange { get; set; } = "default";
    public string DefaultQueue { get; set; } = "default";
    public bool DefaultQueueIsDurable { get; set; } = true;
    public bool DefaultQueueIsExclusive { get; set; } = false;
    public bool DefaultQueueIsAutoDelete { get; set; } = false;
}
