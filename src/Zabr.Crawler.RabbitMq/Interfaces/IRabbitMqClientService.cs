using RabbitMQ.Client;

namespace Zabr.Crawler.RabbitMq.Interfaces;

public interface IRabbitMqClientService : IDisposable
{
    IModel? GetChannel();

    string? GetDefaultQueue();

    void SendMessage(object obj, bool isPersistent = true);

    Task SendMessageAsync(object obj, CancellationToken stoppingToken, bool isPersistent = true);
}
