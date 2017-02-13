using System;

namespace Core.Repository.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyAttribute : Attribute
  {
    public PropertyAttribute(string name = null)
    {
      Name = name;
    }

    public string Name { get; set; }
  }
}