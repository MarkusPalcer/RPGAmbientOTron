using System;
using Newtonsoft.Json;

namespace Core.Repository.Sounds
{
  public class Sound 
  {
    public string Hash { get; set; }

    public string Name { get; set; }

    [JsonIgnore]
    public IObservable<Status> Status { get; internal set; }

    public Sound Clone()
    {
      return new Sound()
      {
        Hash = Hash,
        Name = Name,
        Status = Status
      };
    }
  }
}