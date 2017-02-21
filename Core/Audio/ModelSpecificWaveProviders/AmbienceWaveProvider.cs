using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Repository.Models;
using Core.Util;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Prism.Events;

namespace Core.Audio.ModelSpecificWaveProviders
{
  [Export]
  public class AmbienceWaveProvider : ModelSpecificWaveProvider<AmbienceModel>
  {
    private readonly MixingSampleProvider mixer;

    private readonly Dictionary<AmbienceModel.Entry, ISampleProvider> entrySources = new Dictionary<AmbienceModel.Entry, ISampleProvider>();
    private readonly SampleToWaveProvider output;
    private readonly DynamicVisitor<AmbienceModel.Entry> entryVisitor = new DynamicVisitor<AmbienceModel.Entry>();

    [ImportingConstructor]
    public AmbienceWaveProvider(ExportFactory<LoopWaveProvider> loopSourceExportFactory, IEventAggregator eventAggregator) : base(eventAggregator)
    {
      mixer = new MixingSampleProvider(AudioService.DefaultWaveFormat)
      {
        ReadFully = true
      };

      output = new SampleToWaveProvider(mixer);
      WaveFormat = output.WaveFormat;

      entryVisitor.Register(CreateEntryFactory<Loop, LoopWaveProvider>(loopSourceExportFactory));
    }

    private Action<TModel> CreateEntryFactory<TModel, TWaveProvider>(ExportFactory<TWaveProvider> factory)
      where TWaveProvider : ModelSpecificWaveProvider<TModel>
      where TModel : AmbienceModel.Entry
    {
      return model =>
      {
        var newItem = factory.CreateExport().Value;
        newItem.SetModel(model);
        entrySources[model] = new WaveToSampleProvider(newItem);
        mixer.AddMixerInput(newItem);
      };
    }

    protected override void UpdateFromModel(AmbienceModel model)
    {
      var oldEntries = new HashSet<AmbienceModel.Entry>(entrySources.Keys);
      var newEntries = new HashSet<AmbienceModel.Entry>(model.Entries);

      foreach (var entry in newEntries.ToArray())
      {
        if (oldEntries.Contains(entry) && newEntries.Contains(entry))
        {
          oldEntries.Remove(entry);
          newEntries.Remove(entry);
        }
      }

      foreach (var oldEntry in oldEntries)
      {
        var item = entrySources[oldEntry];
        entrySources.Remove(oldEntry);
        mixer.RemoveMixerInput(item);
      }

      foreach (var newEntry in newEntries)
      {
        entryVisitor.Visit(newEntry);
      }
    }

    public ISampleProvider Audio => mixer;
    public override int Read(byte[] buffer, int offset, int count)
    {
      return output.Read(buffer, offset, count);
    }
  }
}