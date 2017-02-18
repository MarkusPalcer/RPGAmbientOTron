using System.ComponentModel.Composition;
using Core.Repository;
using Core.Repository.Models;
using Core.Repository.Sources;
using NAudio.Wave;

namespace Core.Audio
{
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
}