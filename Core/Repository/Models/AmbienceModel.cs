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

    [SliderProperty]
    public float Volume { get; set; } = 1.0f;

    public abstract class Entry
    {
      public abstract string Name { get; set; }

      [SliderProperty]
      public float Volume { get; set; } = 1.0f;

    }
  }

  [TypeName("Looped Sound")]
  public class LoopModel : AmbienceModel.Entry
  {
    public SoundModel Sound { get; set; }

    [Property]
    [JsonIgnore] 
    public override string Name { get { return Sound.Name; } set { Sound.Name = value; } }

    [Property]
    public bool IsPlaying { get; set; } = true;

    public LoopModel Clone()
    {
      return new LoopModel
      {
        Sound = Sound.Clone()
      };
    }
  }
}