using System.ComponentModel.Composition;
using Core.Audio.ModelSpecificWaveProviders;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Prism.Events;

namespace Core.Audio
{
  [Export(typeof(IAudioService))]
  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class AudioService : IAudioService
  {
    private readonly ExportFactory<AmbienceWaveProvider> ambienceSourceFactory;

    private WaveOut outputDevice;
    private MixingSampleProvider rootMixer;

    public const int TemporarayBufferSize = 1024;

    internal static readonly WaveFormat DefaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
    private readonly SoundBoardWaveProvider soundBoardWaveProvider;

    [ImportingConstructor]
    internal AudioService(IInternalRepository repository, ExportFactory<AmbienceWaveProvider> ambienceSourceFactory, IEventAggregator eventAggregator)
    {
      this.ambienceSourceFactory = ambienceSourceFactory;

      eventAggregator.OnModelAdd<AmbienceModel>(Initialize);
      soundBoardWaveProvider = new SoundBoardWaveProvider(repository, eventAggregator);
    }

    public void Init()
    {
      rootMixer = new MixingSampleProvider(DefaultWaveFormat)
      {
        ReadFully = true
      };

      rootMixer.AddMixerInput(soundBoardWaveProvider);

      outputDevice = new WaveOut();
      outputDevice.Init(rootMixer);
      outputDevice.Play();
    }

    private void Initialize(AmbienceModel model)
    {
      var source = ambienceSourceFactory.CreateExport().Value;
      source.SetModel(model);
      rootMixer.AddMixerInput(source);
    }
  }
}