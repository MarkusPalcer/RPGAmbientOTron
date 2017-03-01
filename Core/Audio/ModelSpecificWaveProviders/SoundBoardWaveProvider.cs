using Core.Audio.NAudio;
using Core.Events;
using Core.Repository;
using Core.Repository.Sounds;
using Futures;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Prism.Events;

namespace Core.Audio.ModelSpecificWaveProviders
{
  internal class SoundBoardWaveProvider : IWaveProvider
  {
    private readonly IInternalRepository repository;
    private readonly IEventAggregator eventAggregator;
    private readonly MixingSampleProvider rootMixer;
    private readonly IWaveProvider waveProviderImplementation;

    public SoundBoardWaveProvider(IInternalRepository repository, IEventAggregator eventAggregator)
    {
      this.repository = repository;
      this.eventAggregator = eventAggregator;

      rootMixer = new MixingSampleProvider(AudioService.DefaultWaveFormat)
      {
        ReadFully = true
      };

      waveProviderImplementation = new SampleToWaveProvider16(rootMixer);

      eventAggregator.GetEvent<TriggerSoundEvent>().Subscribe(Trigger, ThreadOption.BackgroundThread, true);
    }

    private void Trigger(Sound sound)
    {
      var source = repository.GetSource(sound);
      if (source == null)
        return;

      var waveStream = new CompletionReportingWaveStream(source.Open());
      var sampleProvider = waveStream.MakeStereo().ToSampleProvider();
      waveStream.Completion.Subscribe(_ => rootMixer.RemoveMixerInput(sampleProvider));

      rootMixer.AddMixerInput(sampleProvider);
    }

    public int Read(byte[] buffer, int offset, int count)
    {
      return waveProviderImplementation.Read(buffer, offset, count);
    }

    public WaveFormat WaveFormat => waveProviderImplementation.WaveFormat;
  }
}