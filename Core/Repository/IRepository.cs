using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Repository.Models;
using Core.Repository.Sounds;
using Core.Repository.Sources;

namespace Core.Repository
{
  internal interface IInternalRepository
  {
    ISource GetSource(Sound sound);
  }

  public interface IRepository
  {
    SoundBoard LoadSoundBoard(Guid id);
    Task<IEnumerable<SoundBoard>> GetSoundBoards();
    void Add(SoundBoard model);

    Task<Sound> ImportFile(string fileName);
      
    IEnumerable<Cache> GetCaches();

    Task ImportCache(string cacheFolder);
  }
}
