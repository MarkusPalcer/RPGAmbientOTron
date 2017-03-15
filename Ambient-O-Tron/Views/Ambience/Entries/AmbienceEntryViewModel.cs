using AmbientOTron.Views.Navigation;
using Core.Repository.Models;
using Prism.Mvvm;

namespace AmbientOTron.Views.Ambience.Entries
{
  public abstract class AmbienceEntryViewModel : BindableBase
  {
    public abstract string Name { get; }
  }
}