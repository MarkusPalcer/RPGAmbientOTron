using System;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public class StringPropertyViewModel : PropertyViewModel<string>
  {
    public StringPropertyViewModel(Func<string> getter, Action<string> setter) : base(getter, setter) {}
  }
}