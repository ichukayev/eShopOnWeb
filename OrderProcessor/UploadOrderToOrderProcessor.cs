using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace OrderProcessor
{
    public static class UploadOrderToOrderProcessor
    {
        [FunctionName("UploadOrderToOrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "OrderProcessor",
                collectionName: "orders",
                ConnectionStringSetting = "OrderProcessorCosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var orderFile = JsonConvert.DeserializeObject<OrderFile>(requestBody);

            var orderJson = JsonConvert.SerializeObject(orderFile.Order);

            var order = (Order)orderJson;

            string responseMessage = await CreateUserDocumentIfNotExists(client, "OrderProcessor", "orders", orderJson, order.Id);

            return new OkObjectResult(responseMessage);
        }

        private static async Task<string> CreateUserDocumentIfNotExists(DocumentClient client, string databaseName, string collectionName, dynamic order, int orderId)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, orderId.ToString()), new RequestOptions { PartitionKey = new PartitionKey(orderId) });
                return $"Order {orderId} already exists in the database";
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), order);
                    return $"Created order {orderId}";
                }
                else
                {
                    throw;
                }
            }
        }

        private class OrderFile
        {
            public string FileName { get; set; }
            public dynamic Order { get; set; }
        }

        private class Order
        {
            public int Id { get; set; }
        }
    }
}
