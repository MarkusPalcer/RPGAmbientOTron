using System.Reactive;
using System.Threading.Tasks;
using Futures;
using NAudio.Wave;

namespace Core.Audio
{
  internal class CompletionReportingWaveStream : WaveStream
  {
    private readonly WaveStream source;
    private readonly TaskCompletionSource<Unit> taskCompletionSource = new TaskCompletionSource<Unit>();

    public CompletionReportingWaveStream(WaveStream source)
    {
      this.source = source;
      Completion = taskCompletionSource.Task.ToFuture();
    }

    #region Overrides of AudioFileReader

    public override int Read(byte[] buffer, int offset, int count)
    {
      var read = source.Read(buffer, offset, count);

      if (read == 0)
      {
        taskCompletionSource.TrySetResult(Unit.Default);
      }

      return read;
    }

    public override WaveFormat WaveFormat
    {
      get { return source.WaveFormat; }
    }

    public override long Length
    {
      get { return source.Length; }
    }

    public override long Position
    {
      get { return source.Position; }
      set { source.Position = value; }
    }

    #endregion

    public IFuture<Unit> Completion { get; }
  }
}