using System;
using System.Collections.Generic;
using Core.Repository.Models.Sources;

namespace Core.Repository.Models
{

    public class Library 
    {
        public List<SoundBoard> SoundBoards { get; } = new List<SoundBoard>();
    }
}
