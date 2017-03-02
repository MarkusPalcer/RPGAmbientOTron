using System;
using System.Reactive;
using Futures;
using NAudio.Wave;

namespace Core.Audio.Triggerables
{
  public interface ITriggerToken : IDisposable, IFuture<Unit>, ISampleProvider
  {
  }
}