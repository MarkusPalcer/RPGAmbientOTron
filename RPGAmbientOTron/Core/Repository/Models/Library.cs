using System.Collections.Generic;

namespace Core.Repository.Models
{

    public class Library 
    {
        public Core.Persistence.Models.Library PersistenceModel { get; set; }

        public List<AudioFile> Files { get; } = new List<AudioFile>();
    }
}
