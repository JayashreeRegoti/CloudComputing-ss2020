using GaussianAndMeanFilter;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MyExperiment.SEProjectLearningAPI.GaussianAndMeanFilter
{
    public static class RunFilter
    {
        public static double[,,] Run(string inputImageFileName)
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) =>
                GetDataArrayFromImage(inputImageFileName)));

            lApi.AddModule(new GaussianFilter());

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            return result;
        }

        private static double[,,] GetDataArrayFromImage(string inputImageFileName)
        {
            Bitmap inputBitmap = new Bitmap($"{inputImageFileName}");

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
    }
}
