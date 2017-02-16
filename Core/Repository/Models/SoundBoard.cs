using System;
using System.Collections.Generic;
using System.Windows.Media;
using Core.Repository.Attributes;
using Core.Repository.Sounds;
using Newtonsoft.Json;

namespace Core.Repository.Models
{
  [TypeName("Sound Board")]
  public class SoundBoard
  {
    [Property]
    public string Name { get; set; } = @"Unnamed soundboard";

    public List<Entry> Entries { get; set; } = new List<Entry>();

    [TypeName("Sound Board Entry")]
    public class Entry
    {
      public Sound Sound { get; set; }

      [Property]
      [JsonIgnore]
      public string Name
      {
        get { return Sound.Name; }
        set { Sound.Name = value; }
      }

      [Property]
      public Color Color { get; set; } = Colors.Khaki;


    }
  }

}