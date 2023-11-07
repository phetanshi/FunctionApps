using System;
using System.IO;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace FunctionApp3
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([BlobTrigger("psblob/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Size: {myBlob.Length} Bytes");
            try
            {
                StreamReader reader = new StreamReader(myBlob);
                string fileContent = reader.ReadToEnd();
                var lines = fileContent.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
                Repository repository = new Repository();
                bool isInserted = repository.CreateEmployee(lines);

                if (isInserted)
                {
                    log.LogInformation("Data have been inserted");

                    string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("psblob");
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
                    blockBlob.DeleteIfExistsAsync().Wait();

                    log.LogInformation("Data file have been deleted");

                }
                else
                    log.LogInformation("Data does not have been inserted");
            }
            catch (Exception ex)
            {
                log.LogInformation($"\nError : {ex.ToString()}\n");
            }

            log.LogInformation($"Process have been completed");
        }
    }
}
