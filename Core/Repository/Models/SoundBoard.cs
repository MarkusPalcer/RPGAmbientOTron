using System;
using System.Collections.Generic;
using Core.Repository.Sounds;

namespace Core.Repository.Models
{
  public class SoundBoard
  {
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = @"Unnamed soundboard";

    public List<Sound> Sounds { get; set; } = new List<Sound>();
  }
}