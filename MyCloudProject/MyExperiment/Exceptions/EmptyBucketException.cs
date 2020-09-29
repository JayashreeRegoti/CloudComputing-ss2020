using System;

namespace MyExperiment.Exceptions
{
  public class EmptyBucketException : System.Exception
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public EmptyBucketException(String message)
      : base(message)
    {

    }
  }
}