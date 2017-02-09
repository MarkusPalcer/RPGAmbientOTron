using AmbientOTron.Views.Navigation;
using Core.Repository.Sounds;

namespace AmbientOTron.Views.Sounds
{
  public class SoundNavigationViewModel : NavigationItemViewModelWithoutChildren<Sound>
  {
    public override void SetModel(Sound newModel)
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