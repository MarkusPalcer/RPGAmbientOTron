using System.Collections.Generic;

namespace Core.Persistence.Models
{
    public class Library
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public List<AudioFile> Files { get; } = new List<AudioFile>();

        public List<string> SatteliteLibraries { get; } = new List<string>();
    }
}