using System;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Core.Audio
{
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