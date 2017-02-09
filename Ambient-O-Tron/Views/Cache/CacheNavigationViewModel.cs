using AmbientOTron.Views.Navigation;

namespace AmbientOTron.Views.Cache
{
  public class CacheNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.Cache>
  {
    public override void SetModel(Core.Repository.Models.Cache newModel)
    {
      Model = newModel;
      UpdateFromModel();
    }

    private void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}