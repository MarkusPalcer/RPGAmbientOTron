using System.Collections.Generic;

namespace Core.Repository.Models
{

    public class Library 
    {
        public List<SoundBoard> SoundBoards { get; } = new List<SoundBoard>();
        public List<string> Files { get; } = new List<string>();
    }
}
