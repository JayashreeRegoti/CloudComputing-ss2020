﻿namespace LearningFoundation
{
  /// <summary>
  /// Defines interface for lading and saving of pipeline.
  /// </summary>
  public interface IModelPersistenceProvider
  {
    void Save(string name, LearningApi api);

    LearningApi Load(string name);
  }
}