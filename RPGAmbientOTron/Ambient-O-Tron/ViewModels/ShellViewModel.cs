using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace AmbientOTron.ViewModels
{
  [Export]
  public class ShellViewModel : Prism.Mvvm.BindableBase
  {
    public ShellViewModel()
    {
      SoundEffects = new ObservableCollection<SoundEffectViewModel>();
    }

    public ObservableCollection<SoundEffectViewModel> SoundEffects { get; }
  }
}
