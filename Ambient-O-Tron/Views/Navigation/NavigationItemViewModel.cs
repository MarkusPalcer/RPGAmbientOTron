using System.Windows.Input;
using Prism.Mvvm;

namespace AmbientOTron.Views.Navigation
{
  public abstract class NavigationItemViewModel<TModel> : BindableBase
  {
    protected TModel Model;

    public abstract void SetModel(TModel newModel);

    private string name;

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    private ICommand navigateCommand;

    public ICommand NavigateCommand
    {
      get { return navigateCommand; }
      protected set { SetProperty(ref navigateCommand, value); }
    }


  }
}