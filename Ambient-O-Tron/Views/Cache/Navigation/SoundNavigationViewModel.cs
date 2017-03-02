using AmbientOTron.Views.Navigation;
using Core.Repository.Sounds;

namespace AmbientOTron.Views.Cache.Navigation
{
  public class SoundNavigationViewModel : NavigationItemViewModel<SoundModel>
  {
    protected override void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}