using System.Collections.Generic;
using Core.Repository.Sounds;
using Newtonsoft.Json;

namespace Core.Repository.Models
{
  public class Cache
  {
    public string Name { get; set; }

    public string Folder { get; set; }

    [JsonIgnore]
    public List<SoundModel> Sounds { get; } = new List<SoundModel>();

    protected bool Equals(Cache other)
    {
      return string.Equals(Folder, other.Folder);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((Cache) obj);
    }

    public override int GetHashCode()
    {
      return Folder?.GetHashCode() ?? 0;
    }
  }
}