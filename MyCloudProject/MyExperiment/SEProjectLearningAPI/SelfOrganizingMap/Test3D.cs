using LearningFoundation;

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SelfOrganizingMap
{
  public class Test3D
  {
    readonly static string path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));

    //MZ: converted the original unit test into a static method
    public static void SelfOrganizingMapTest_3DData_To2D()
    {
      List<string> labels = new List<string>();

      var api = new LearningApi();
      api.UseActionModule<List<double[]>, List<double[]>>((data, context) =>
      {
        List<double[]> patterns = new List<double[]>();
        var dimensions = 3;
        StreamReader reader = File.OpenText(path + "\\SelfOrganizingMap\\RGB.csv");
              ///<Summary>Ignore first line.
              reader.ReadLine();
        while (!reader.EndOfStream)
        {
          string[] line = reader.ReadLine().Split(',');
          labels.Add(line[0]);
          double[] inputs = new double[dimensions];

          for (int i = 0; i < dimensions; i++)
          {
            inputs[i] = double.Parse(line[i + 1]);
          }
          patterns.Add(inputs);
        }
        reader.Dispose();

        return patterns;
      });
      api.AddModule(new Map(3, 15, 0.000001));
      var r = api.Run() as Neuron[];

      //creating new stringbuilder object for saving the result output in the csv file
      var csv = new StringBuilder();

      for (int i = 0; i < r.Length; i++)
      {
        System.Diagnostics.Debug.WriteLine("{0},{1},{2}", labels[i], r[i].m_X, r[i].m_Y);

        //saving each row data in stringbuilder object csv
        string First = labels[i].ToString();
        string Second = r[i].m_X.ToString();
        string Third = r[i].m_Y.ToString();
        string newLine = string.Format("{0},{1},{2}", First, Second, Third);
        csv.AppendLine(newLine);
      }

      //Writing the 2D mapping data into the output file
      File.WriteAllText(path + "\\SelfOrganizingMap\\RGB_2Doutput.csv", csv.ToString());
    }

    public static string SelfOrganizingMapTest_3D_2D(string PathFilename)
    {
      List<string> labels = new List<string>();

      var api = new LearningApi();
      api.UseActionModule<List<double[]>, List<double[]>>((data, context) =>
      {
        List<double[]> patterns = new List<double[]>();
        var dimensions = 3;
        StreamReader reader = File.OpenText(PathFilename);
              ///<Summary>Ignore first line.
              reader.ReadLine();
        while (!reader.EndOfStream)
        {
          string[] line = reader.ReadLine().Split(',');
          labels.Add(line[0]);
          double[] inputs = new double[dimensions];

          for (int i = 0; i < dimensions; i++)
          {
            inputs[i] = double.Parse(line[i + 1]);
          }
          patterns.Add(inputs);
        }
        reader.Dispose();

        return patterns;
      });
      api.AddModule(new Map(3, 15, 0.000001));
      var r = api.Run() as Neuron[];

      //creating new stringbuilder object for saving the result output in the csv file
      var csv = new StringBuilder();

      for (int i = 0; i < r.Length; i++)
      {
        System.Diagnostics.Debug.WriteLine("{0},{1},{2}", labels[i], r[i].m_X, r[i].m_Y);

        //saving each row data in stringbuilder object csv
        string First = labels[i].ToString();
        string Second = r[i].m_X.ToString();
        string Third = r[i].m_Y.ToString();
        string newLine = string.Format("{0},{1},{2}", First, Second, Third);
        csv.AppendLine(newLine);
      }

      return csv.ToString();

      //Writing the 2D mapping data into the output file
      //File.WriteAllText(path + "\\SelfOrganizingMap\\RGB_2Doutput.csv", csv.ToString());
    }
  }
}