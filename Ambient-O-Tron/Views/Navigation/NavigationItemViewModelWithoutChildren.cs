using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Mvvm;

namespace AmbientOTron.Views.Navigation
{
  public abstract class NavigationItemViewModel<TModel, TChildren> : BindableBase, INavigationEntry<TChildren>
  {
    private string name;

    public string Name
    {
      get { return name; }
      protected set { SetProperty(ref name, value); }
    }

    private ObservableCollection<TChildren> items = new ObservableCollection<TChildren>();

    public ObservableCollection<TChildren> Items
    {
      get { return items; }
      protected set { SetProperty(ref items, value); }
    }

    private ICommand navigateCommand = null;

    public ICommand NavigateCommand
    {
      get { return navigateCommand; }
      protected set { SetProperty(ref navigateCommand, value); }
    }

    public TModel Model;

    public abstract void SetModel(TModel newModel);
  }

  public abstract class NavigationItemViewModelWithoutChildren<TModel> : NavigationItemViewModel<TModel, object>
  {
  }

  
}