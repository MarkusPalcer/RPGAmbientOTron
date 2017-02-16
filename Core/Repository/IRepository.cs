using System;
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

    void Add(SoundBoard model);

    Task<Sound> ImportFile(string fileName);
      
    Task ImportCache(string cacheFolder);
    void Add(Ambience newModel);
  }
}
