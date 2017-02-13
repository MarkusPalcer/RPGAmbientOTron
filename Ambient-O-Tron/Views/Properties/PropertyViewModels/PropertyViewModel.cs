using System;
using Prism.Mvvm;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public abstract class PropertyViewModel : BindableBase
  {
    private string name;

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    public Action SendModelUpdate { get; set; }

    public abstract void Update();
  }

  public abstract class PropertyViewModel<TProperty> : PropertyViewModel
  {
    private Action<TProperty> setter;
    private Func<TProperty> getter;

    private TProperty value;

    public PropertyViewModel(Func<TProperty> getter, Action<TProperty> setter)
    {
      this.getter = getter;
      this.setter = setter;
    }

    public TProperty Value
    {
      get { return value; }
      set
      {
        if (SetProperty(ref this.value, value))
        {
          setter(value);
          SendModelUpdate?.Invoke();
        }
      }
    }
    public override void Update()
    {
      value = getter();
      OnPropertyChanged(() => Value);
    }
  }
}