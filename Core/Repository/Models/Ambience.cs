using System.Collections.Generic;
using Core.Repository.Attributes;
using Core.Repository.Sounds;
using Newtonsoft.Json;

namespace Core.Repository.Models
{
  public interface IAmbienceEntryVisitor
  {
    void Visit(Loop model);
  }

  public class Ambience
  {
    [Property]
    public string Name { get; set; }

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public abstract class Entry
    {
      public abstract string Name { get; set; }

      public abstract void Accept(IAmbienceEntryVisitor visitor);
    }
  }

  [TypeName("Looped Sound")]
  public class Loop : Ambience.Entry
  {
    public Sound Sound { get; set; }

    [Property]
    [JsonIgnore] 
    public override string Name { get { return Sound.Name; } set { Sound.Name = value; } }

    public override void Accept(IAmbienceEntryVisitor visitor)
    {
      visitor.Visit(this);
    }

    public Loop Clone()
    {
      return new Loop
      {
        Sound = Sound.Clone()
      };
    }
  }
}