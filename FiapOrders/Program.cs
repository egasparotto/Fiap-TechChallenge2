using FiapOrders.Configuration;

using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(ServicesConfiguration.Configure)
    .Build();

host.Run();
