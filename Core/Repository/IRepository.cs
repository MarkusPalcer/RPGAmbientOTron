using System;
using System.Collections.Generic;
using Core.Repository.Models;
using Core.Repository.Models.Sources;

namespace Core.Repository
{
  public interface IRepository
  {
    SoundBoard LoadSoundBoard(Guid id);
    IEnumerable<SoundBoard> GetSoundBoards();
    void Save(SoundBoard model);

    AudioFile GetSource(string fileName);

  }
}
