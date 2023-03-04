using Domain;
using CrossCutting;
using Microsoft.Extensions.Hosting;
using Serverless_Api.Middlewares;
using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serverless_Api.Extensions;

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddEventStore();
        services.AddDomainDependencies();
        services.InjectUseCases();
        services.AddLogging(b => b.AddConsole());
        var serviceProvider = services.BuildServiceProvider();

        StaticLoggerFactory.Initialize(serviceProvider.GetRequiredService<ILoggerFactory>());
    })
    .ConfigureFunctionsWorkerDefaults(builder => builder.UseMiddleware<AuthMiddleware>())
    .Build();

host.Run();
