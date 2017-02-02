using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Prism.Mvvm;

namespace AmbientOTron.Views.Navigation
{
  [Export]
  public class NavigationViewModel : BindableBase
  {
    [ImportingConstructor]
    public NavigationViewModel([ImportMany(typeof(NavigationGroup<>))] IEnumerable<object> groups)
    {
      Groups = new ObservableCollection<object>(groups);
    }

    public ObservableCollection<object> Groups { get; }
  }
}