using System.Threading.Tasks;
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
    Task<Sound> ImportFile(string fileName);
      
    Task ImportCache(string cacheFolder);
  }
}
