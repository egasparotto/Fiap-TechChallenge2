using FiapOrders.Domain.Entities.Orders;
using FiapOrders.DTO;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

using System.Net;

namespace FiapOrders.Functions.Orders
{
    public class AprovePaymentEvent
    {
        [Function("AprovePayment")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AprovePayment/{instanceId}")] HttpRequestData req,
            string instanceId,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {

            try
            {
                bool isApproved = true;

                await client.RaiseEventAsync(instanceId, "AprovePayment", isApproved);
                var instance = await client.GetInstanceAsync(instanceId);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    success = true
                });

                return response;
            }
            catch
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Pagamento não localizado ou já confirmado.");
                return response;
            }

        }
    }
}
