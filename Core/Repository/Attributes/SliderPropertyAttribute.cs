using System;

namespace Core.Repository.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class SliderPropertyAttribute : PropertyAttribute
  {
    public float Minimum { get; set; } = 0.0f;
    public float Maximum { get; set; } = 1.0f;

    public float LargeChange { get; set; } = 0.1f;
  }
}