using Microsoft.Extensions.Logging;

using MyCloudProject.Common;

using MyExperiment;
using MyExperiment.CloudStorages;

using System;
using System.Threading;

namespace MyCloudProject
{
  class Program
  {
    /// <summary>
    /// Your project ID from the last semester.
    /// </summary>
    private static string projectName = "ML19/20-3.13. Validate and improve Tests of Existing Algorithm: Gaussian and Mean algorithm";

    static void Main(string[] args)
    {
      CancellationTokenSource tokeSrc = new CancellationTokenSource();

      Console.CancelKeyPress += (sender, e) =>
      {
        e.Cancel = true;
        tokeSrc.Cancel();
      };

      Console.WriteLine($"Started experiment: {projectName}");

      //init configuration
      var cfgRoot = Common.InitHelpers.InitConfiguration(args);

      var cfgSec = cfgRoot.GetSection("MyConfig");

      // InitLogging
      var logFactory = InitHelpers.InitLogging(cfgRoot);
      var logger = logFactory.CreateLogger("Train.Console");

      logger?.LogInformation($"{DateTime.Now} -  Started experiment: {projectName}");

      IStorageProvider storageProvider = new AzureBlobOperations(cfgSec);

      Experiment experiment = new Experiment(cfgSec, storageProvider, logger/* put some additional config here */);

      experiment.RunQueueListener(tokeSrc.Token).Wait();

      logger?.LogInformation($"{DateTime.Now} -  Experiment exit: {projectName}");
    }

  }
}
