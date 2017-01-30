using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Repository.Models
{
  public class SoundBoard
  {
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = @"Unnamed soundboard";

    public List<AudioFile> Sounds { get; set; } = new List<AudioFile>();
  }
}