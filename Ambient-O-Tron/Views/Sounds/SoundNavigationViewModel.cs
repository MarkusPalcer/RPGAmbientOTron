using AmbientOTron.Views.Navigation;
using Core.Repository.Sounds;

namespace AmbientOTron.Views.Sounds
{
  public class SoundNavigationViewModel : NavigationItemViewModelWithoutChildren<Sound>
  {
    protected override void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}