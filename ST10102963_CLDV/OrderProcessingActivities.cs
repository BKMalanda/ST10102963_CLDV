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

    /* Code reference: https://www.youtube.com/watch?v=Un0wxOfSSTE */
    /* Code reference: https://www.youtube.com/watch?v=ttfSvYiS83A */
    /* Code reference: https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=csharp */

    public static class OrderProcessingActivities
    {
        [FunctionName("UpdateInventory")]
        public static string UpdateInventory([ActivityTrigger] OrdersViewModel order, ILogger log)
        {
            log.LogInformation($"Updating inventory for order {order.OrderID}");
            // Implement inventory update logic here
            return $"Inventory updated for order {order.OrderID}";
        }

        [FunctionName("ProcessPayment")]
        public static string ProcessPayment([ActivityTrigger] OrdersViewModel order, ILogger log)
        {
            log.LogInformation($"Processing payment for order {order.OrderID}");
            // Implement payment processing logic here
            return $"Payment processed for order {order.OrderID}";
        }

        [FunctionName("ConfirmOrder")]
        public static string ConfirmOrder([ActivityTrigger] OrdersViewModel order, ILogger log)
        {
            log.LogInformation($"Confirming order {order.OrderID}");
            // Implement order confirmation logic here
            return $"Order {order.OrderID} confirmed";
        }
    }
}
