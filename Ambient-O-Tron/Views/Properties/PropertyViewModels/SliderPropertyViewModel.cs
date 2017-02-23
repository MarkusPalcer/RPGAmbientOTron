using System;
using System.Reflection;
using Core.Repository.Attributes;

namespace AmbientOTron.Views.Properties.PropertyViewModels
{
  public class SliderPropertyViewModel : PropertyViewModel<float>
  {
    public SliderPropertyViewModel(Func<float> getter, Action<float> setter) : base(getter, setter) {}

    public static SliderPropertyViewModel Create(PropertyInfo property, object model, SliderPropertyAttribute attribute)
    {
      return new SliderPropertyViewModel(() => (float) property.GetValue(model), x => property.SetValue(model, x))
      {
        Minimum = attribute.Minimum,
        Maximum = attribute.Maximum,
        LargeChange = attribute.LargeChange,
        Name = attribute.Name ?? property.Name
      };
    }

    public float Minimum { get; set; }

    public float Maximum { get; set; }  

    public float LargeChange { get; set; }
  }
}