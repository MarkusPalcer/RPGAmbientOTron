using System.ComponentModel.Composition;
using Core.Audio.ModelSpecificWaveProviders;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Core.Repository.Sounds;
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
    private readonly IInternalRepository repository;
    private readonly ExportFactory<AmbienceWaveProvider> ambienceSourceFactory;

    private readonly NoPlayback noPlayback = new NoPlayback();

    private WaveOut outputDevice;
    private MixingSampleProvider rootMixer;

    public const int TemporarayBufferSize = 1024;

    internal static WaveFormat DefaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

    [ImportingConstructor]
    internal AudioService(IInternalRepository repository, ExportFactory<AmbienceWaveProvider> ambienceSourceFactory, IEventAggregator eventAggregator)
    {
      this.repository = repository;
      this.ambienceSourceFactory = ambienceSourceFactory;

      eventAggregator.OnModelAdd<AmbienceModel>(Initialize);
    }

    #region Implementation of IAudioService

    public IPlayback Play(Sound sound)
    {
      var source = repository.GetSource(sound);

      if (source == null)
        return noPlayback;

      return new Playback(source.Open());
    }

    #endregion


    public void Init()
    {
      rootMixer = new MixingSampleProvider(DefaultWaveFormat)
      {
        ReadFully = true
      };

      outputDevice = new WaveOut();
      outputDevice.Init(rootMixer);
      outputDevice.Play();
    }

    private void Initialize(AmbienceModel model)
    {
      var source = ambienceSourceFactory.CreateExport().Value;
      source.SetModel(model);
      rootMixer.AddMixerInput(source.Audio);
    }
  }
}