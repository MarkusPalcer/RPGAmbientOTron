using NAudio.Wave;

namespace Core.Audio
{
  public interface IAudioSource
  {
    WaveStream Audio { get; }
  }
}