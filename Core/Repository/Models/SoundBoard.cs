using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Repository.Models
{
  public class SoundBoard
  {
    [JsonIgnore]
    public string Path { get; set; }
    public string Name { get; set; }
    public List<AudioFile> Sounds { get; set; } = new List<AudioFile>();
  }
}