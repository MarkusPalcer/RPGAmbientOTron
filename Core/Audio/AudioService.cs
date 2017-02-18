using System.ComponentModel.Composition;
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
  public class AudioService : IAudioService
  {
    private readonly IInternalRepository repository;
    private readonly ExportFactory<AmbienceSource> ambienceSourceFactory;

    private readonly NoPlayback noPlayback = new NoPlayback();

    private readonly WaveOut outputDevice;
    private readonly WaveMixerStream32 rootMixer;


    [ImportingConstructor]
    internal AudioService(IInternalRepository repository, ExportFactory<AmbienceSource> ambienceSourceFactory, IEventAggregator eventAggregator)
    {
      this.repository = repository;
      this.ambienceSourceFactory = ambienceSourceFactory;

      rootMixer = new WaveMixerStream32
      {
        AutoStop = false
      };

      outputDevice = new WaveOut();
      outputDevice.Init(rootMixer);

      eventAggregator.OnModelAdd<Ambience>(Initialize);
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


    private void Initialize(Ambience model)
    {
      var source = ambienceSourceFactory.CreateExport().Value;
      source.SetModel(model);
      rootMixer.AddInputStream(source.Audio);
    }
  }
}