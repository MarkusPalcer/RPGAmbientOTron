using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Core.Repository.Sounds;
using NAudio.Wave;

namespace Core.Audio
{
  [Export]
  public class LoopWaveProvider : IWaveProvider
  {
    private readonly IInternalRepository repository;
    private IWaveProvider waveProviderImplementation;
    private WaveStream sourceStream;
    private byte[] internalBuffer;
    private readonly SemaphoreSlim semaphore =new SemaphoreSlim(1);

    [ImportingConstructor]
    internal LoopWaveProvider(IInternalRepository repository)
    {
      this.repository = repository;
      internalBuffer = new byte[AudioService.TemporarayBufferSize];
    }

    public void SetModel(Loop model)
    {
      model.Sound.Status.DistinctUntilChanged().Where(x => x == Status.Ready).Subscribe(_ => Dispatcher.CurrentDispatcher.Invoke(() => ReloadSound(model.Sound)));
    }

    private void ReloadSound(Sound sound)
    {
      using (semaphore.Protect())
      {
        sourceStream = repository.GetSource(sound).Open();

        var stereo = sourceStream.WaveFormat.Channels == 1
                       ? new MonoToStereoProvider16(sourceStream)
                       : (IWaveProvider) sourceStream;

        waveProviderImplementation = new Wave16ToFloatProvider(stereo);
      }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
      using (semaphore.Protect())
      {
        if (waveProviderImplementation == null)
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

    public WaveFormat WaveFormat => AudioService.DefaultWaveFormat;
  }
}