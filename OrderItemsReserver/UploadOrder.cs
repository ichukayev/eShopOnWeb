using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Text;

namespace OrderItemsReserver
{
    public static class UploadOrder
    {
        [FunctionName("UploadOrder")]
        [FixedDelayRetry(3, "00:00:10")]
        public static async Task Run(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")] string queueOrder,
            [StorageAccount("AzureWebJobsStorage")] CloudStorageAccount storageAccount,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {queueOrder}");

            var order = JsonConvert.DeserializeObject<Order>(queueOrder);

            var fileName = $"Order-{order.Id}.json";

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("orders");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(queueOrder)));
        }

        private class Order
        {
            public int Id { get; set; }
        }
    }

    
}
