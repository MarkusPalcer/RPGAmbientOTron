using System.Collections.Generic;
using Core.Repository.Models;

namespace Core.Repository
{
    public interface IRepository
    {
        AudioFile GetAudioFileModel(string fileName);
        IEnumerable<Library> Libraries { get; }
    }
}