
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Core.Audio.Triggerables;
using Core.Audio.Triggers;
using Core.Events;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Core.Repository.Sounds;
using Core.Util;
using Futures;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Prism.Events;

namespace Core.Audio.ModelSpecificWaveProviders
{
  internal class SoundBoardWaveProvider : IWaveProvider
  {
    private readonly IInternalRepository repository;
    private readonly IEventAggregator eventAggregator;
    private readonly MixingSampleProvider rootMixer;
    private readonly IWaveProvider waveProviderImplementation;

    private readonly SerialDisposable modelUpdateDisposable = new SerialDisposable();
    private readonly SerialDisposable modelRemoveDisposable = new SerialDisposable();

    private readonly DynamicVisitor<SoundBoardModel.Entry> triggerCreationVisitor = new DynamicVisitor<SoundBoardModel.Entry>();

    private class TriggerData
    {
      public IDisposable Subscription;
      public readonly HashSet<ITriggerToken> TriggeredSounds = new HashSet<ITriggerToken>();
    }

    private readonly Dictionary<SoundBoardModel.Entry, ITrigger> triggers = new Dictionary<SoundBoardModel.Entry, ITrigger>();
    private readonly Dictionary<ITrigger, TriggerData> triggerData = new Dictionary<ITrigger, TriggerData>();

    private void LoadNewSoundBoard(SoundBoardModel model)
    {
      triggerData.Keys.ToArray().ForEach(RemoveTrigger);
      triggers.Clear();

      if (model == null)
        return;

      model.Entries.ForEach(triggerCreationVisitor.Visit);
      modelUpdateDisposable.Disposable = eventAggregator.OnModelUpdate(model, UpdateModel);
      modelRemoveDisposable.Disposable = eventAggregator.OnModelRemove(model, _ => LoadNewSoundBoard(null));
    }

    private void UpdateModel(SoundBoardModel model)
    {
      var oldTriggers = new Dictionary<SoundBoardModel.Entry, ITrigger>(triggers);
      triggers.Clear();

      foreach (var entry in model.Entries)
      {
        if (oldTriggers.ContainsKey(entry))
        {
          triggers[entry] = oldTriggers[entry];
          oldTriggers.Remove(entry);
        }
        else
        {
          triggerCreationVisitor.Visit(entry);
        }
      }

      foreach (var trigger in oldTriggers.Values)
      {
        RemoveTrigger(trigger);
      }
    }

    private void AddTriggerForSound(SoundBoardModel.Entry model)
    {
      var trigger = new ManualTrigger<SoundModel>(eventAggregator, model.Sound, new SoundTriggerable(repository.GetSource(model.Sound)));
      AddTrigger(trigger);
      triggers[model] = trigger;
    }

    private void RemoveTrigger(ITrigger trigger)
    {
      var data = triggerData[trigger];
      data.Subscription.Dispose();
      data.TriggeredSounds.ForEach(x => x.Dispose());
      triggerData.Remove(trigger);
      trigger.Dispose();
    }

    private void AddTrigger(ITrigger trigger)
    {
      var data = new TriggerData();
      triggerData[trigger] = data;
      data.Subscription = trigger.Subscribe(item => Trigger(item, trigger));
    }

    private void Trigger(ITriggerToken item, ITrigger trigger)
    {
      triggerData[trigger].TriggeredSounds.Add(item);
      rootMixer.AddMixerInput(item);
      item.Subscribe(
        _ =>
        {
          rootMixer.RemoveMixerInput(item);
          item.Dispose();
          if (triggerData.ContainsKey(trigger))
          {
            triggerData[trigger].TriggeredSounds.Remove(item);
          }
        });
    }

    public SoundBoardWaveProvider(IInternalRepository repository, IEventAggregator eventAggregator)
    {
      this.repository = repository;
      this.eventAggregator = eventAggregator;

      rootMixer = new MixingSampleProvider(AudioService.DefaultWaveFormat)
      {
        ReadFully = true
      };

      waveProviderImplementation = new SampleToWaveProvider16(rootMixer);

      triggerCreationVisitor.Register<SoundBoardModel.Entry>(AddTriggerForSound);

      this.eventAggregator.GetEvent<InitializeSoundBoardEvent>()
          .Subscribe(LoadNewSoundBoard, ThreadOption.BackgroundThread, true);
    }


    public int Read(byte[] buffer, int offset, int count)
    {
      return waveProviderImplementation.Read(buffer, offset, count);
    }

    public WaveFormat WaveFormat => waveProviderImplementation.WaveFormat;
  }
}