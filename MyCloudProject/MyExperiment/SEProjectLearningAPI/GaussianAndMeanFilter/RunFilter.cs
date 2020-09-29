using GaussianAndMeanFilter;

using LearningFoundation;

using Microsoft.Extensions.Logging;

using System;
using System.Drawing;
using System.IO;

namespace MyExperiment.SEProjectLearningAPI.GaussianAndMeanFilter
{
  public static class RunFilter
  {
    public static string Run(string localStorageFilePath, ILogger logger)
    {
      var inputFileName = Path.GetFileNameWithoutExtension(localStorageFilePath);
      var imageExtention = Path.GetExtension(localStorageFilePath);

      LearningApi lApi = new LearningApi();
      lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) =>
          GetDataArrayFromImage(localStorageFilePath)));

      lApi.AddModule(new GaussianFilter());

      lApi.AddModule(new MeanFilter());

      logger?.LogInformation($"Applying Gaussian and Mean Filter...");
      double[,,] result = lApi.Run() as double[,,];

      var resultBitmap = GenerateResultBitmap(result);
      var resultImage = (Image)resultBitmap;

      var resultFileName = $"{inputFileName}_output_{Guid.NewGuid()}{imageExtention}";
      var localStorageResultLocation = Path.Combine(Experiment.DataFolder, resultFileName);
      resultImage.Save(localStorageResultLocation);

      logger?.LogInformation($"Local Storage Result file link: {localStorageResultLocation}");

      return resultFileName;
    }

    private static double[,,] GetDataArrayFromImage(string localStorageFilePath)
    {
      Bitmap inputBitmap = new Bitmap(localStorageFilePath);

      double[,,] data = new double[inputBitmap.Width, inputBitmap.Height, 3];

      for (int x = 0; x < inputBitmap.Width; x++)
      {
        for (int y = 0; y < inputBitmap.Height; y++)
        {
          Color pixelColor = inputBitmap.GetPixel(x, y);

          data[x, y, 0] = pixelColor.R;
          data[x, y, 1] = pixelColor.G;
          data[x, y, 2] = pixelColor.B;
        }
      }
      return data;
    }

    private static Bitmap GenerateResultBitmap(double[,,] result)
    {
      Bitmap resultBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

      for (int x = 0; x < result.GetLength(0); x++)
      {
        for (int y = 0; y < result.GetLength(1); y++)
        {
          Color pixelColor = Color.FromArgb((int)result[x, y, 0], (int)result[x, y, 1], (int)result[x, y, 2]);
          resultBitmap.SetPixel(x, y, pixelColor);
        }
      }

      return resultBitmap;
    }
  }
}
