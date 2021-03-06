using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using Core.Audio.NAudio;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Core.Repository.Sounds;
using NAudio.Wave;
using Prism.Events;

namespace Core.Audio.ModelSpecificWaveProviders
{
  [Export]
  public class LoopWaveProvider : ModelSpecificWaveProvider<LoopModel>
  {
    private readonly IInternalRepository repository;
    private IWaveProvider waveProviderImplementation;
    private WaveStream sourceStream;
    private readonly byte[] internalBuffer;
    private readonly SemaphoreSlim semaphore =new SemaphoreSlim(1);
    private VolumeWaveProvider16 volumeLimiter;

    [ImportingConstructor]
    internal LoopWaveProvider(IInternalRepository repository, IEventAggregator eventAggregator) : base(eventAggregator)
    {
      this.repository = repository;
      internalBuffer = new byte[AudioService.TemporarayBufferSize];
      WaveFormat = AudioService.DefaultWaveFormat;
    }

    public override void SetModel(LoopModel model)
    {
      base.SetModel(model);
      model.Sound.Status.DistinctUntilChanged().Where(x => x == Status.Ready).Subscribe(_ => Dispatcher.CurrentDispatcher.Invoke(() => ReloadSound(model.Sound)));
    }

    private void ReloadSound(SoundModel sound)
    {
      using (semaphore.Protect())
      {
        sourceStream = repository.GetSource(sound).Open();

        var stereo = sourceStream.MakeStereo();

        volumeLimiter = new VolumeWaveProvider16(stereo)
        {
          Volume = Model.Volume
        };
        
        waveProviderImplementation = new Wave16ToFloatProvider(volumeLimiter);
      }
    }

    protected override void UpdateFromModel(LoopModel model)
    {
      base.UpdateFromModel(model);
      if (volumeLimiter != null) volumeLimiter.Volume = model.Volume;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      using (semaphore.Protect())
      {
        if (waveProviderImplementation == null || !Model.IsPlaying)
        {
          Array.Clear(buffer, offset, count);
          return count;
        }

        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
          var bytesToRead = Math.Min(count - totalBytesRead, 1024);
          var bytesRead = waveProviderImplementation.Read(internalBuffer, 0, bytesToRead);
          Array.Copy(internalBuffer, 0, buffer, offset + totalBytesRead, bytesRead);

          if (bytesRead == 0)
          {
            if (sourceStream.Position == 0)
            {
              // something wrong with the source stream
              break;
            }
            // loop
            sourceStream.Position = 0;
          }
          totalBytesRead += bytesRead;
        }
        return totalBytesRead;
      }
    }
  }
}