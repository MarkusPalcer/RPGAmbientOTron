using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Core.Audio.ModelSpecificWaveProviders;
using Core.Audio.NAudio;
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
    private readonly IEventAggregator eventAggregator;

    private WaveOut outputDevice;
    private MixingSampleProvider rootMixer;

    public const int TemporarayBufferSize = 1024;

    internal static readonly WaveFormat DefaultWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
    private readonly SoundBoardWaveProvider soundBoardWaveProvider;
    private readonly Dictionary<AmbienceModel, Tuple<ISampleProvider, IDisposable[]>> ambienceEntries = new Dictionary<AmbienceModel, Tuple<ISampleProvider, IDisposable[]>>();

    [ImportingConstructor]
    internal AudioService(IInternalRepository repository, ExportFactory<AmbienceWaveProvider> ambienceSourceFactory, IEventAggregator eventAggregator)
    {
      this.ambienceSourceFactory = ambienceSourceFactory;
      this.eventAggregator = eventAggregator;

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
      var export = ambienceSourceFactory.CreateExport();

      var source = export.Value;
      source.SetModel(model);
      var stream = source.MakeStereo().ToSampleProvider();
      rootMixer.AddMixerInput(stream);

      ambienceEntries[model] = Tuple.Create(
        stream, 
        new IDisposable[] {
          export,
          eventAggregator.OnModelRemove(model, Remove, ThreadOption.BackgroundThread)
        });
    }

    private void Remove(AmbienceModel model)
    {
      var entry = ambienceEntries[model];
      ambienceEntries.Remove(model);

      rootMixer.RemoveMixerInput(entry.Item1);

      entry.Item2.ForEach(x => x.Dispose());
    }
  }
}