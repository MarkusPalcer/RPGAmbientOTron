using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AmbientOTron.Views.Navigation
{
  public interface INavigationEntry
  {
    string Name { get; }

    ICommand NavigateCommand { get; }
    IEnumerable<NavigationItemContextMenuEntry> ContextMenuEntries { get; }
  }

  public interface INavigationEntry<TChildren> : INavigationEntry
  {
    
    ObservableCollection<TChildren> Items { get; }
    
  }
}