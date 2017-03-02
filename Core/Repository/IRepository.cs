using System.Threading.Tasks;
using Core.Repository.Sounds;
using Core.Repository.Sources;

namespace Core.Repository
{
  internal interface IInternalRepository
  {
    ISource GetSource(SoundModel sound);
  }

  public interface IRepository
  {
    Task<SoundModel> ImportFile(string fileName);
      
    Task ImportCache(string cacheFolder);
  }
}
