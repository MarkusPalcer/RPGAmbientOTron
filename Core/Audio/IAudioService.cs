using Core.Repository.Models.Sources;

namespace Core.Audio
{
  public interface IAudioService
  {
    IPlayback PlayAudioFile(AudioFile path);
  }
} 