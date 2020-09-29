namespace LearningFoundation
{
  public interface IPipelineModule
  {

  }

  public interface IPipelineModule<TIN, TOUT> : IPipelineModule
  {
    TOUT Run(TIN data, IContext ctx);
  }
}
