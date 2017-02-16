using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Mvvm;

namespace AmbientOTron.Views.Navigation
{
  public interface IWithModel
  {
    object Model { get; }
  }

  public interface IWithModel<out TModel>  : IWithModel
  {
    new TModel Model { get; }
  }

  public abstract class NavigationItemViewModel<TModel, TChildren> : BindableBase, INavigationEntry<TChildren>, IWithModel<TModel>
    where TModel : class
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

    protected abstract void UpdateFromModel();

    protected virtual void OnModelSet(TModel newModel) { }

    private TModel model = null;

    public TModel Model
    {
      get { return model; }
      set
      {
        model = value;
        OnModelSet(value);
        UpdateFromModel();
      }
    }

    object IWithModel.Model => model;
  }

  public abstract class NavigationItemViewModel<TModel> : NavigationItemViewModel<TModel, object>
    where TModel : class
  {
  }

  
}