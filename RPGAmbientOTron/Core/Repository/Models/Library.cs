using System;
using System.Collections.Generic;

namespace Core.Repository.Models
{

    public class Library 
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public List<Library> SatteliteLibraries { get; } = new List<Library>();

        public List<AudioFile> Files { get; } = new List<AudioFile>();
    }
}
