using System;
using System.Reactive.Subjects;
using Core.Audio.Triggerables;
using Core.Events;
using Prism.Events;

namespace Core.Audio.Triggers
{
  public class ManualTrigger<TModel> : ITrigger
  {
    private readonly Subject<ITriggerToken> subject = new Subject<ITriggerToken>();
    private readonly SubscriptionToken eventSubscription;

    public ManualTrigger(IEventAggregator eventAggregator, TModel model, ITriggerable triggerable)
    {
      eventSubscription = eventAggregator.GetEvent<ManualTriggerEvent<TModel>>()
                                         .Subscribe(
                                           _ => subject.OnNext(triggerable.Trigger()),
                                           ThreadOption.BackgroundThread,
                                           true,
                                           m => ReferenceEquals(m, model));
    }

    public IDisposable Subscribe(IObserver<ITriggerToken> observer)
    {
      return subject.Subscribe(observer);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        eventSubscription.Dispose();
        subject.OnCompleted();
      }

      // Release unmanaged resources here
    }
  }
}
