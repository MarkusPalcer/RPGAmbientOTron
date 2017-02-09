using System.Threading.Tasks;
using Core.Repository.Sounds;
using Core.Repository.Sources;

namespace Core.Audio
{
  public interface IAudioService
  {
    IPlayback Play(Sound model);
  }
} 