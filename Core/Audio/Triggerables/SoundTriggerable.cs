using Core.Audio.NAudio;
using Core.Repository.Sources;
using NAudio.Wave;

namespace Core.Audio.Triggerables
{
  internal class SoundTriggerable : ITriggerable
  {
    private readonly ISource source;

    public SoundTriggerable(ISource source)
    {
      this.source = source;
    }

    public ITriggerToken Trigger()
    {
      var waveStream = new CompletionReportingWaveStream(source.Open());
      return new TriggerToken(waveStream.Completion, waveStream.MakeStereo().ToSampleProvider());
    }
  }
}