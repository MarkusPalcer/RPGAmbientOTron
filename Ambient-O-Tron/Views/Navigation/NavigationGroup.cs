using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AmbientOTron.Views.Navigation
{
  public class NavigationGroup<TItem> : INavigationEntry<TItem>
  {
    public string Name { get; set; }
    public ObservableCollection<TItem> Items { get; set; }
    public ICommand NavigateCommand { get; } = null;

    public IEnumerable<NavigationItemContextMenuEntry> ContextMenuEntries { get; protected set; } = null;
  }
}