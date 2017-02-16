using System.Collections.Generic;

namespace Core.Repository.Models
{

    public class Library 
    {
        public List<SoundBoard> SoundBoards { get; } = new List<SoundBoard>();
        public List<string> Files { get; } = new List<string>();
    public List<Cache> Caches { get; set; } = new List<Cache>();

    public List<Ambience> Ambiences { get; set; } = new List<Ambience>();
  }
}
