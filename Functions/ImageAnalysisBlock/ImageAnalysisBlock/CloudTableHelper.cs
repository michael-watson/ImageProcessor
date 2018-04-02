using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using ImageAnalysisBlock.Models;

namespace ImageAnalysisBlock.Functions
{
    public static class CloudTableHelper
    {
        static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(EnvironmentVariables.StorageAccountConnection);
        static CloudTableClient cloudClient = storageAccount.CreateCloudTableClient();
        static CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        public static CloudTable PendingNotificationTable = cloudClient.GetTableReference(nameof(PendingNotification));

        public static async Task<TableResult> Delete(string tag, string notificationId)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<PendingNotification>(tag, notificationId);
            TableResult retrievedResult = PendingNotificationTable.Execute(retrieveOperation);
            PendingNotification deleteEntity = (PendingNotification)retrievedResult.Result;

            if (deleteEntity != null)
            {
                return await PendingNotificationTable.ExecuteAsync(TableOperation.Delete(deleteEntity));
            }

            return null;
        }
    }
}