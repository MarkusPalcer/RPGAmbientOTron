using System;
using System.Collections.Generic;
using Core.Repository.Models;

namespace Core.Repository
{
  public interface IRepository
  {
    IEnumerable<Library> Libraries { get; }
    AudioFile GetAudioFileModel(string fileName);
    void Save(Library model);
    void LoadLibrary(string path);
    Library GetLibraryModel(string path);
    void Save(AudioFile model);
    SoundBoard LoadSoundBoard(Guid id);
    IEnumerable<SoundBoard> GetSoundBoards();
    void Save(SoundBoard model);
  }
}
