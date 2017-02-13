using System;
using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public class StringPropertyViewModel : PropertyViewModel<string>
  {
    public StringPropertyViewModel(Func<string> getter, Action<string> setter) : base(getter, setter) {}

    [Export]
    public static PropertiesViewModel.KnownPropertyType Declaration
    {
      get
      {
        return new PropertiesViewModel.KnownPropertyType
        {
          Type = typeof(string),
          ViewModelFactory =
            (property, model) =>
              new StringPropertyViewModel(() => (string) property.GetValue(model), x => property.SetValue(model, x))
        };
      }
    }
  }
}