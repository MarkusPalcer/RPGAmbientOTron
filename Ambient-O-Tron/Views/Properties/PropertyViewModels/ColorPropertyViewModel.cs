using System;
using System.Windows.Media;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public class ColorPropertyViewModel : PropertyViewModel<Color>
  {
    public ColorPropertyViewModel(Func<Color> getter, Action<Color> setter) : base(getter, setter) {}
  }
}