using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using MyCloudProject.Common;

using MyExperiment.CloudStorages;
using MyExperiment.SEProjectLearningAPI.GaussianAndMeanFilter;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyExperiment
{
  public class Experiment : IExperiment
  {
    public static string DataFolder { get; private set; }
    private IStorageProvider storageProvider;
    private ILogger logger;
    private MyConfig config;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="configSection"></param>
    /// <param name="storageProvider"></param>
    /// <param name="log"></param>
    public Experiment(IConfigurationSection configSection, IStorageProvider storageProvider, ILogger log)
    {
      this.storageProvider = storageProvider;
      logger = log;
      config = new MyConfig();
      configSection.Bind(config);

      //  Creates the directory where the input-data from the blob will be stored
      DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
          config.LocalPath);
      Directory.CreateDirectory(DataFolder);
    }

    /// <summary>
    /// Runs the Experiment
    /// </summary>
    /// <param name="localStorageFilePath"></param>
    /// <returns></returns>
    public async Task<ExperimentResult> Run(string localStorageFilePath)
    {
      var startTime = DateTime.UtcNow;

      // running Gaussian and Mean Filter
      var nameOfExperiment = "Gaussian and Mean Filter";
      string resultFileName = RunFilter.Run(localStorageFilePath, logger);

      var endTime = DateTime.UtcNow;

      // Step 2: Uploading output to blob storage
      var uploadedUri =
          await storageProvider.UploadResultFile(resultFileName, null);

      var uploadUriAsString = Encoding.ASCII.GetString(uploadedUri);

      logger?.LogInformation(
          $"Test cases output file uploaded successful. Blob URL: {uploadUriAsString}");

      long duration = endTime.Subtract(startTime).Seconds;

      var res = new ExperimentResult(this.config.GroupId, Guid.NewGuid().ToString());
      UpdateExperimentResult(res, startTime, endTime, duration, nameOfExperiment, localStorageFilePath, uploadUriAsString);

      return res;
    }

    /// <inheritdoc/>
    public async Task RunQueueListener(CancellationToken cancelToken)
    {
      CloudQueue queue = await AzureQueueOperations.CreateQueueAsync(config, logger);

      while (cancelToken.IsCancellationRequested == false)
      {
        var message = await queue.GetMessageAsync(cancelToken);
        try
        {
          if (message != null)
          {
            // STEP 1. Reading message from Queue and deserializing
            var experimentRequestMessage =
                JsonConvert.DeserializeObject<ExerimentRequestMessage>(message.AsString);
            logger?.LogInformation(
                $"Received message from the queue with experimentID: " +
                $"{experimentRequestMessage.ExperimentId}, " +
                $"description: {experimentRequestMessage.Description}, " +
                $"name: {experimentRequestMessage.Name}");

            // STEP 2. Downloading the input file from the blob storage
            var fileToDownload = experimentRequestMessage.InputFile;
            var localStorageFilePath = await storageProvider.DownloadInputFile(fileToDownload);

            logger?.LogInformation(
                $"File download successful. Downloaded file link: {localStorageFilePath}");

            // STEP 3. Applying Gaussian and Mean Filter on input image file
            var mresult = await Run(localStorageFilePath);

            mresult.InputFileUrl = experimentRequestMessage.InputFile;
            mresult.Description = experimentRequestMessage.Description;

            var mresultAsString = JsonConvert.SerializeObject(mresult);
            var mresultAsByte = Encoding.UTF8.GetBytes(mresultAsString);

            // STEP 4. Uploading result file to blob storage
            var muploadedUri =
                await storageProvider.UploadResultFile("ResultFile-" + Guid.NewGuid() + ".csv",
                    mresultAsByte);
            logger?.LogInformation($"Uploaded result file on blob");
            mresult.SeExperimentOutputBlobUrl
                = Encoding.ASCII.GetString(muploadedUri);

            // STEP 5. Uploading result file to table storage
            await storageProvider.UploadExperimentResult(mresult);

            // STEP 6. Deleting the message from the queue
            await queue.DeleteMessageAsync(message, cancelToken);
          }
        }
        catch (Exception ex)
        {
          logger?.LogError(ex, "Caught an exception: {0}", ex.Message);
          await queue.DeleteMessageAsync(message, cancelToken);
        }

        // pause
        await Task.Delay(500, cancelToken);
      }

      logger?.LogInformation("Cancel pressed. Exiting the listener loop.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="experimentResult"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="duration"></param>
    /// <param name="downloadFileUrl"></param>
    /// <param name="testCaseOutputUri"></param>
    private static void UpdateExperimentResult(ExperimentResult experimentResult, DateTime startTime,
        DateTime endTime, long duration, string name, string downloadFileUrl, string testCaseOutputUri)
    {
      experimentResult.StartTimeUtc = startTime;
      experimentResult.EndTimeUtc = endTime;
      experimentResult.DurationSec = duration;
      experimentResult.Name = name;
      experimentResult.InputFileUrl = downloadFileUrl;
      experimentResult.SeExperimentOutputFileUrl = testCaseOutputUri;
    }
  }
}