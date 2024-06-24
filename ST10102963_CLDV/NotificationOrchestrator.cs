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
    public static class NotificationOrchestrator
    {

        /* Code reference: https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=csharp */

        [FunctionName("NotificationOrchestrator")]
        public static async Task RunOrchestrator(
       [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var order = context.GetInput<OrdersViewModel>();

            await context.CallActivityAsync("SendOrderReceivedNotification", order);
            await context.CallActivityAsync("SendOrderProcessingNotification", order);
            await context.CallActivityAsync("SendOrderShippedNotification", order);
        }

        [FunctionName("SendOrderReceivedNotification")]
        public static void SendOrderReceivedNotification([ActivityTrigger] OrdersViewModel order, ILogger log)
        {
            log.LogInformation($"Sending order received notification for order {order.OrderID}");
            // Implement notification logic here
        }

        [FunctionName("SendOrderProcessingNotification")]
        public static void SendOrderProcessingNotification([ActivityTrigger] OrdersViewModel order, ILogger log)
        {
            log.LogInformation($"Sending order processing notification for order {order.OrderID}");
            // Implement notification logic here
        }

        [FunctionName("SendOrderShippedNotification")]
        public static void SendOrderShippedNotification([ActivityTrigger] OrdersViewModel order, ILogger log)
        {
            log.LogInformation($"Sending order shipped notification for order {order.OrderID}");
            // Implement notification logic here
        }

    }
}
