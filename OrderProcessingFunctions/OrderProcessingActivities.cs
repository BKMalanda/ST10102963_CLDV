using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingFunctions
{
    public class OrderProcessingActivities
    {
        [FunctionName("UpdateInventory")]
        public async Task<string> UpdateInventory(
             [ActivityTrigger] string parameters,
             ILogger log)
        {
            // Replace with inventory update logic
            log.LogInformation($"Updating inventory for order: {parameters}");
            return "Inventory updated";
        }

        [FunctionName("ProcessPayment")]
        public async Task<string> ProcessPayment(
            [ActivityTrigger] string parameters,
            ILogger log)
        {
            // Replace with payment processing logic
            log.LogInformation($"Processing payment for order: {parameters}");
            return "Payment processed";
        }

        [FunctionName("SendOrderConfirmation")]
        public async Task<string> SendOrderConfirmation(
            [ActivityTrigger] string parameters,
            ILogger log)
        {
            // Replace with order confirmation logic
            log.LogInformation($"Sending order confirmation for order: {parameters}");
            return "Order confirmation sent";
        }
    }
}
