using FiapOrders.Domain.Entities.Mails;
using FiapOrders.Domain.Entities.Orders;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

using System.Text;

namespace FiapOrders.Functions.Orders
{
    public static class OrchestrateFiapOrder
    {
        [Function(nameof(OrchestrateFiapOrder))]
        public static async Task<Order> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context, Order order)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(OrchestrateFiapOrder));

            var paymentNotificationEmail = CreatePaymentNotificationMail(context.InstanceId, order);
            await context.CallActivityAsync("UserPaymentNotification", paymentNotificationEmail);

            using var timeoutCts = new CancellationTokenSource();
            var timeToAproval = context.CurrentUtcDateTime.AddMinutes(5);
            var durableTimeout = context.CreateTimer(timeToAproval, timeoutCts.Token);

            var approvalEvent =  context.WaitForExternalEvent<bool>("AprovePayment");
            if (approvalEvent == await Task.WhenAny(approvalEvent, durableTimeout))
            {
                timeoutCts.Cancel();
                var successPaymentNotification = CreatePaymentSuccessNotificationMail(order);
                await context.CallActivityAsync("UserPaymentNotification", successPaymentNotification);
            }
            else
            {
                var errrorPaymentNotification = CreatePaymentErrorNotificationMail(order);
                await context.CallActivityAsync("UserPaymentNotification", errrorPaymentNotification);
            }


            return order;
        }


        private static Mail CreatePaymentNotificationMail(string instanceId, Order order)
        {
            var aprrovePaymentUrl = $"https://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/AprovePayment/{instanceId}";
            var sb = new StringBuilder();
            sb.AppendLine($"Olá {order.Name},<br>");
            sb.AppendLine("Recebemos o seu pedido em nossa plataforma<br>");
            sb.AppendLine("Para confirmar o seu pagamento clique no link abaixo<br>");
            sb.AppendLine($"<a href=\"{aprrovePaymentUrl}\">{aprrovePaymentUrl}</a>");

            return new Mail()
            {
                Message = sb.ToString(),
                Subject = "Fiap Tech Challenge 2 - Confirme seu pagamento",
                To = order.Email
            };
        }

        private static Mail CreatePaymentSuccessNotificationMail(Order order)
        {
            return new Mail()
            {
                Message = order.ToString(),
                Subject = "Fiap Tech Challenge 2 - Pagamento confirmado",
                To = order.Email
            };
        }

        private static Mail CreatePaymentErrorNotificationMail(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Olá {order.Name},<br>");
            sb.AppendLine("Lamentamos que você perdeu o prazo para efetuar o pagamento<br>");
            sb.AppendLine("Seu pedido foi cancelado");
            return new Mail()
            {
                Message = sb.ToString(),
                Subject = "Fiap Tech Challenge 2 - Pagamento com erro",
                To = order.Email
            };
        }


    }
}
