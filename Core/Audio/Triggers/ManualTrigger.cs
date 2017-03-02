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

    public ManualTrigger(IEventAggregator eventAggregator, TModel model, ITriggerable triggerable)
    {
      eventAggregator.GetEvent<ManualTriggerEvent<TModel>>()
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
  }
}
