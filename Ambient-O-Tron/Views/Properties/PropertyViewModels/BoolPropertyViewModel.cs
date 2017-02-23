using System;
using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public class BoolPropertyViewModel : PropertyViewModel<bool>
  {
    public BoolPropertyViewModel(Func<bool> getter, Action<bool> setter) : base(getter, setter) { }

    [Export]
    public static PropertiesViewModel.KnownPropertyType Declaration
    {
      get
      {
        return new PropertiesViewModel.KnownPropertyType
        {
          Type = typeof(bool),
          ViewModelFactory =
            (property, model) =>
              new BoolPropertyViewModel(() => (bool)property.GetValue(model), x => property.SetValue(model, x))
        };
      }
    }
  }
}