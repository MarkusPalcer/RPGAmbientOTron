using System;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public class ColorPropertyViewModel : PropertyViewModel<Color>
  {
    public ColorPropertyViewModel(Func<Color> getter, Action<Color> setter) : base(getter, setter) { }

    [Export]
    public static PropertiesViewModel.KnownPropertyType Declaration
    {
      get
      {
        return new PropertiesViewModel.KnownPropertyType
        {
          Type = typeof(Color),
          ViewModelFactory =
            (property, model) =>
              new ColorPropertyViewModel(() => (Color)property.GetValue(model), x => property.SetValue(model, x))
        };
      }
    }
  }
}
