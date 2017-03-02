using Core.Navigation;
using Prism.Regions;

namespace Core.Extensions
{
  public static class NavigationExtensions
  {
    public const string ModelKey = "model";

    public static TModel GetModel<TModel>(this NavigationContext navigationContext)
    {
      return (TModel)navigationContext.Parameters[ModelKey];
    }

    public static NavigationParameters WithModel<TModel>(this NavigationParameters navigationParameters, TModel model)
    {
      navigationParameters.Add(ModelKey, model);
      return navigationParameters;
    }
  }
}