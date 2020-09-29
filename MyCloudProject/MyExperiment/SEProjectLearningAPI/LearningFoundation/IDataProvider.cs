
using System.Collections.Generic;

namespace LearningFoundation
{
  /// <summary>
  /// Defines the status of the trained model.
  /// </summary>
  public interface IDataProvider<TOut> : IEnumerator<TOut>, IPipelineModule<object, TOut[]>
  {
  }
}
