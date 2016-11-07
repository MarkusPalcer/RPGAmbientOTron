using System.Reactive;
using System.Threading.Tasks;

namespace Core.Audio
{
  public interface IAudioService
  {
    Task PlayAudioFile(string path);
  }
} 