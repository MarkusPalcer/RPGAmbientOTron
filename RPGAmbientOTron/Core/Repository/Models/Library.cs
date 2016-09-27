using System;
using System.Collections.Generic;

namespace Core.Repository.Models
{

    public class Library 
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Core.Persistence.Models.Library PersistenceModel { get; set; }

        public List<AudioFile> Files { get; } = new List<AudioFile>();
    }
}
