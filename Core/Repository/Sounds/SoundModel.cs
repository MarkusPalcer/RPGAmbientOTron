using System;
using Newtonsoft.Json;

namespace Core.Repository.Sounds
{
  public class SoundModel 
  {
    public string Hash { get; set; }

    public string Name { get; set; }

    [JsonIgnore]
    public IObservable<Status> Status { get; internal set; }

    public SoundModel Clone()
    {
      return new SoundModel()
      {
        Hash = Hash,
        Name = Name,
        Status = Status
      };
    }
  }
}