using System;
using Core.Audio.Triggerables;

namespace Core.Audio.Triggers
{
  public interface ITrigger : IObservable<ITriggerToken>, IDisposable
  {
  }
}