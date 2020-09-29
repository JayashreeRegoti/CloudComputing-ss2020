using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;

namespace MyExperiment.CloudStorages
{
    public static class AzureQueueOperations
    {
        /// <summary>
        /// Create a queue for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudQueue object</returns>
        public static async Task<CloudQueue> CreateQueueAsync(MyConfig config, ILogger logger)
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount =
                CreateStorageAccountFromConnectionString(config.StorageConnectionString, logger);

            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(config.Queue);
            try
            {
                await queue.CreateIfNotExistsAsync();
                logger?.LogInformation(
                    $"Created a queue for the demo");
            }
            catch(Exception exception)
            {
                logger?.LogInformation(
                    $"Caught an exception while creating a queue {0}", exception.Message);
                throw;
            }
            return queue;
        }

        #region Private Methods
        
        /// <summary>
        /// Validate the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string</param>
        /// <returns>CloudStorageAccount object</returns>
        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString, ILogger logger)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException exception)
            {
                logger?.LogInformation(
                    $"Caught an exception while parsing the connection string {0}", exception.Message);
                throw;
            }
            catch (ArgumentException exception)
            {
                logger?.LogInformation(
                    $"Caught an exception while parsing the connection string {0}", exception.Message);
                throw;
            }
            return storageAccount;
        }

        #endregion
    }
}