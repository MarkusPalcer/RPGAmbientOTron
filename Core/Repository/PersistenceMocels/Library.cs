using System.Collections.Generic;
using Core.Repository.Models;

namespace Core.Repository.PersistenceMocels
{
    public class Library
    {
        public string Name { get; set; }

        public List<AudioFile> Files { get; } = new List<AudioFile>();

        public List<string> SatteliteLibraryPaths { get; } = new List<string>();

    public List<SoundBoard> SoundBoards { get; } = new List<SoundBoard>();
  }
}