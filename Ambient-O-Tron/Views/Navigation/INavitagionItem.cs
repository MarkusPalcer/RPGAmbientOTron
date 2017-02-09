using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AmbientOTron.Views.Navigation
{
  public interface INavigationEntry<TChildren>
  {
    string Name { get; }
    ObservableCollection<TChildren> Items { get; }
    ICommand NavigateCommand { get; }
  }
}