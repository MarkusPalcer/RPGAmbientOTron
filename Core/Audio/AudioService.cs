using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Repository;
using Core.Repository.Models;
using Core.Repository.Sounds;
using Core.Repository.Sources;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

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
    internal AudioService(IInternalRepository repository, ExportFactory<AmbienceSource> ambienceSourceFactory)
    {
      this.repository = repository;
      this.ambienceSourceFactory = ambienceSourceFactory;

      rootMixer = new WaveMixerStream32
      {
        AutoStop = false
      };

      outputDevice = new WaveOut();
      outputDevice.Init(rootMixer);
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

  public interface IAudioSource
  {
    WaveStream Audio { get; }
  }

  [Export]
  public class LoopSource : IAudioSource
  {
    private readonly IInternalRepository repository;

    private class LoopingWaveStream : WaveStream
    {
      private readonly WaveStream source;

      public LoopingWaveStream(ISource src)
      {
        // TODO: Make disposable and ensure stream is closed upon cleanup
        source = new Mp3FileReader(src.Open());
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        var usedCount = 0;

        while (usedCount < count)
        {
          var used = source.Read(buffer, offset, count - usedCount);
          usedCount += used;
          offset += used;

          if (usedCount < count)
          {
            source.Position = 0;
          }
        }

        return usedCount;
      }

      public override WaveFormat WaveFormat => source.WaveFormat;

      public override long Length => source.Length;

      public override long Position
      {
        get { return source.Position; }
        set { /* NOP */ }
      }
    }

    private LoopingWaveStream sourceStream;

    [ImportingConstructor]
    internal LoopSource(IInternalRepository repository)
    {
      this.repository = repository;
    }

    public void SetModel(Loop model)
    {
     sourceStream = new LoopingWaveStream(repository.GetSource(model.Sound)); 
    }

    public WaveStream Audio => sourceStream;
  }

  [Export]
  public class AmbienceSource : IAudioSource
  {
    private readonly ExportFactory<LoopSource> loopSourceExportFactory;
    private readonly WaveMixerStream32 mixer;

    private readonly Dictionary<Ambience.Entry, IAudioSource> entrySources = new Dictionary<Ambience.Entry, IAudioSource>();

    [ImportingConstructor]
    public AmbienceSource(ExportFactory<LoopSource> loopSourceExportFactory)
    {
      this.loopSourceExportFactory = loopSourceExportFactory;
      mixer = new WaveMixerStream32
      {
        AutoStop = false,
      };
    }

    public void SetModel(Ambience model)
    {
      foreach (var loops in model.Entries.OfType<Loop>())
      {
        var newItem = loopSourceExportFactory.CreateExport().Value;
        entrySources[loops] = newItem;
        mixer.AddInputStream(newItem.Audio);
      }
    }

    public WaveStream Audio => mixer;
  }

  public class NoPlayback : IPlayback
  {
    public TaskAwaiter GetAwaiter()
    {
      return Task.Delay(TimeSpan.Zero).GetAwaiter();
    }

    public void Stop()
    {
    }

    public IObservable<double> Progress { get; } = new BehaviorSubject<double>(100.0);
  }
}