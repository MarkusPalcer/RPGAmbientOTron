using System;

namespace Core.Repository.Attributes
{
  public class TypeNameAttribute : Attribute
  {
    public string Name { get; set; }

    public TypeNameAttribute(string name)
    {
      Name = name;
    }
  }
}