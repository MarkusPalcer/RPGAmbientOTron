using NAudio.Wave;

namespace Core.Audio.NAudio
{
  public static class WaveProviderExtensions
  {
    public static IWaveProvider MakeStereo(this IWaveProvider sourceStream)
    {
      return sourceStream.WaveFormat.Channels == 1
                       ? new MonoToStereoProvider16(sourceStream)
                       : sourceStream;
    }
  }
}