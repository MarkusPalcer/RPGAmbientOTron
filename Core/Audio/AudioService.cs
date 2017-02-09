using System;
using System.ComponentModel.Composition;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Repository;
using Core.Repository.Sounds;

namespace Core.Audio
{
  [Export(typeof(IAudioService))]
  public class AudioService : IAudioService
  {
    private readonly IInternalRepository repository;

    private readonly NoPlayback noPlayback = new NoPlayback();

    [ImportingConstructor]
    internal AudioService(IInternalRepository repository)
    {
      this.repository = repository;
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