using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingFunctions
{
    public class OrderOrchestrator
    {
        [FunctionName("OrderOrchestrator")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace with your business logic
            outputs.Add(await context.CallActivityAsync<string>("UpdateInventory", "parameters"));
            outputs.Add(await context.CallActivityAsync<string>("ProcessPayment", "parameters"));
            outputs.Add(await context.CallActivityAsync<string>("SendOrderConfirmation", "parameters"));

            return outputs;
        }
    }
}