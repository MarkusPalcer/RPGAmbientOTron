using System.Collections.Generic;
using Core.Repository.Attributes;
using Core.Repository.Sounds;
using Newtonsoft.Json;

namespace Core.Repository.Models
{
  public class AmbienceModel
  {
    [Property]
    public string Name { get; set; }

    public List<Entry> Entries { get; set; } = new List<Entry>();

    [JsonIgnore]
    public bool IsPlaying { get; set; } = false;

    public abstract class Entry
    {
      public abstract string Name { get; set; }
    }
  }

  [TypeName("Looped Sound")]
  public class Loop : AmbienceModel.Entry
  {
    public Sound Sound { get; set; }

    [Property]
    [JsonIgnore] 
    public override string Name { get { return Sound.Name; } set { Sound.Name = value; } }

    [Property]
    public bool IsPlaying { get; set; } = true;

    public Loop Clone()
    {
      return new Loop
      {
        Sound = Sound.Clone()
      };
    }
  }
}