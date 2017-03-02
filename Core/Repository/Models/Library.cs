using System.Collections.Generic;

namespace Core.Repository.Models
{

    public class Library 
    {
        public List<SoundBoardModel> SoundBoards { get; } = new List<SoundBoardModel>();
        public List<string> Files { get; } = new List<string>();
    public List<Cache> Caches { get; set; } = new List<Cache>();

    public List<AmbienceModel> Ambiences { get; set; } = new List<AmbienceModel>();
  }
}
