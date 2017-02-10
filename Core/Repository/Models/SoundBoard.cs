using System;
using System.Collections.Generic;
using System.Windows.Media;
using Core.Repository.Sounds;

namespace Core.Repository.Models
{
  public class SoundBoard
  {
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = @"Unnamed soundboard";

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public class Entry
    {
      public Sound Sound { get; set; }

      public Color Color { get; set; } = Colors.Khaki;
    }
  }

}