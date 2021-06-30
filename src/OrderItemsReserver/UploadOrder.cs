using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver
{
    public static class UploadOrder
    {
        [FunctionName("UploadOrder")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "AzureStorageAccount")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
