using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zabr.Crawler.RabbitMq.Interfaces;
using Zabr.Crawler.RabbitMq.Options;
using Zabr.Crawler.RabbitMq.Services;

namespace Zabr.Crawler.RabbitMq.Extensions;

public static class RabbitMqExtension
{
    private static IServiceCollection AddOptions(IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions();

        services.AddSingleton<IOptionsChangeTokenSource<RabbitMqOptions>>(
            new ConfigurationChangeTokenSource<RabbitMqOptions>(string.Empty, configurationSection));

        return services.AddSingleton<IConfigureOptions<RabbitMqOptions>>(
            new NamedConfigureFromConfigurationOptions<RabbitMqOptions>(string.Empty, configurationSection, _ => { }));
    }

    public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        AddOptions(services, configurationSection);

        return services.AddSingleton<IRabbitMqClientService, RabbitMqClientService>();
    }
    
    public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        AddOptions(services, configurationSection);

        return services.AddSingleton<IRabbitMqClientService, RabbitMqClientService>();
    }
}
