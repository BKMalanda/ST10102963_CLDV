using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using ST10102963_CLDV.Models;

namespace ST10102963_CLDV
{
    public static class OrderOrchestrator
    {

        /* Code reference: https://www.youtube.com/watch?v=Un0wxOfSSTE */
        /* Code reference: https://www.youtube.com/watch?v=ttfSvYiS83A */
        /* Code reference: https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=csharp */

        [FunctionName("OrderOrchestrator")]
        public static async Task<List<string>> RunOrchestrator(
         [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Get the order details
            var order = context.GetInput<OrdersViewModel>();

            // Update inventory
            outputs.Add(await context.CallActivityAsync<string>("UpdateInventory", order));

            // Process payment
            outputs.Add(await context.CallActivityAsync<string>("ProcessPayment", order));

            // Confirm order
            outputs.Add(await context.CallActivityAsync<string>("ConfirmOrder", order));

            // Send notifications
            await context.CallSubOrchestratorAsync("NotificationOrchestrator", order);

            return outputs;
        }

    }
}
