namespace Core.Audio
{
  public interface IAudioService
  {
    IPlayback PlayAudioFile(string path);
  }
} 