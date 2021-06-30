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
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [StorageAccount("AzureWebJobsStorage")] CloudStorageAccount storageAccount,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var orderFile = JsonConvert.DeserializeObject<OrderFile>(requestBody);

            var orderJson = JsonConvert.SerializeObject(orderFile.Order);

            string fileName = orderFile.FileName;

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("order-files");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(orderJson)));

            string responseMessage = $"File {fileName} has uploaded";

            return new OkObjectResult(responseMessage);
            
            
        }
    }

    public class OrderFile
    {
        public string FileName { get; set; }
        public dynamic Order { get; set; }
    }
}
