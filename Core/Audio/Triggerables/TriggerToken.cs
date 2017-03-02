using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Futures;
using NAudio.Wave;

namespace Core.Audio.Triggerables
{
  internal class TriggerToken : ITriggerToken
  {
    private readonly ISampleProvider sampleProvider;

    private readonly IDisposable disposable;
    private readonly IFuture<Unit> future;


    public TriggerToken(IFuture<Unit> sourceFuture, ISampleProvider sampleProvider)
    {
      this.sampleProvider = sampleProvider;

      var tcs = new TaskCompletionSource<Unit>();

      disposable = new CompositeDisposable
      {
        sourceFuture.Subscribe(_ => Dispose(), _ => Dispose()),
        Disposable.Create(() => tcs.SetResult(Unit.Default))
      };

      future = tcs.Task.ToFuture();
    }


    public void Dispose()
    {
      disposable.Dispose();
    }

    public int Read(float[] buffer, int offset, int count)
    {
      return sampleProvider.Read(buffer, offset, count);
    }

    public WaveFormat WaveFormat
    {
      get { return sampleProvider.WaveFormat; }
    }

    public IDisposable Subscribe(IFutureObserver<Unit> observer)
    {
      return future.Subscribe(observer);
    }
  }
}
