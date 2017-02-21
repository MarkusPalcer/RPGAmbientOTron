using NAudio.Wave;

namespace Core.Audio
{
  public interface IAudioSource
  {
    ISampleProvider Audio { get; }
  }
}