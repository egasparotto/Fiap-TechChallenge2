using System.Net;
using System.Text.Json;
using FiapOrders.Domain.Entities.Orders;
using FiapOrders.DTO;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FiapOrders.Functions.Orders
{
    public class FiapOrderHttpTriger
    {
        [Function("NewOrder")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [FromBody] OrderDTO orderDTO,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("FiapOrderHttpTriger");

            var erros = Validate(orderDTO);
            if(erros.Any())
            {
                var responseError = req.CreateResponse(HttpStatusCode.BadRequest);
                await responseError.WriteStringAsync(string.Join(Environment.NewLine, erros));
                return responseError;
            }

            var itens = orderDTO?.Items.Select(x => new Item()
            {
                Description = x.Description,
                Price = x.Price,
                Quantity = x.Quantity
            });

            var order = new Order()
            {
                Name = orderDTO?.Name,
                Email = orderDTO?.Email,
                Items = itens
            };

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(OrchestrateFiapOrder), order);

            logger.LogInformation("Recebido o pedido com o ID '{instanceId}'.", instanceId);

            var aprrovePaymentUrl = $"https://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/AprovePayment/{instanceId}";
;

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                order,
                aprrovePaymentUrl
            });

            return response;
        }

        private static IEnumerable<string> Validate(OrderDTO orderDTO)
        {
            var result = new List<string>();
            if(string.IsNullOrEmpty(orderDTO?.Name))
            {
                result.Add("O Nome deve ser informado.");
            }

            if(string.IsNullOrEmpty(orderDTO?.Email))
            {
                result.Add("O E-Mail deve ser informado.");
            }

            if (orderDTO.Items == null || !orderDTO.Items.Any())
            {
                result.Add("Deve ser informado ao menos um item");
            }
            else
            {
                
                foreach(var item in orderDTO.Items)
                {
                    var pos = orderDTO.Items.IndexOf(item) + 1;

                    if (string.IsNullOrEmpty(item.Description))
                    {
                        result.Add($"Deve ser informada a descrição do item na posição {pos}");
                    }

                    if(item.Quantity <= 0)
                    {
                        result.Add($"A quantidade do item {pos} - {item.Description} deve ser maior que 0");
                    }

                    if (item.Price <= 0)
                    {
                        result.Add($"O valor do item {pos} - {item.Description} deve ser maior que 0");
                    }
                }
            }

            return result;

        }
    }
}
