using FiapOrders.Domain.Entities.Mails;
using FiapOrders.Domain.Interfaces.Mails;
using FiapOrders.Infra;
using FiapOrders.Infra.Mails;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace FiapOrders.Configuration
{
    public static class ServicesConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();

            services.Configure<SmtpConfig>(config =>
            {
                config.From = Environment.GetEnvironmentVariable("FROM_SMTP");
                config.Port = int.Parse(Environment.GetEnvironmentVariable("PORT_SMTP"));
                config.Host = Environment.GetEnvironmentVariable("HOST_SMTP");
                config.User = Environment.GetEnvironmentVariable("USER_SMTP");
                config.Password = Environment.GetEnvironmentVariable("PASSWORD_SMTP");
            });

            services.AddSingleton<ISmtpMailSender, SmtpMailSender>();
        }
    }
}
