using System.ComponentModel.Composition;
using AmbientOTron.Views.Navigation;
using Core.Repository.Models;

namespace AmbientOTron.Views.Ambience.Navigation
{
  [Export]
  public class LoopNavigationViewModel : NavigationItemViewModel<Loop>
  {
    protected override void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}