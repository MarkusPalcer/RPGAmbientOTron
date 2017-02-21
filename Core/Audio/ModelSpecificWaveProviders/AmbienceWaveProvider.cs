using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Extensions;
using Core.Repository.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Prism.Events;

namespace Core.Audio.ModelSpecificWaveProviders
{
  [Export]
  public class AmbienceWaveProvider : IWaveProvider
  {
    private readonly ExportFactory<LoopWaveProvider> loopSourceExportFactory;
    private readonly IEventAggregator eventAggregator;
    private readonly MixingSampleProvider mixer;

    private readonly Dictionary<Ambience.Entry, ISampleProvider> entrySources = new Dictionary<Ambience.Entry, ISampleProvider>();
    private readonly SampleToWaveProvider output;

    [ImportingConstructor]
    public AmbienceWaveProvider(ExportFactory<LoopWaveProvider> loopSourceExportFactory, IEventAggregator eventAggregator)
    {
      this.loopSourceExportFactory = loopSourceExportFactory;
      this.eventAggregator = eventAggregator;
      mixer = new MixingSampleProvider(AudioService.DefaultWaveFormat)
      {
        ReadFully = true
      };


      output = new SampleToWaveProvider(mixer);
    }

    public void SetModel(Ambience model)
    {
      UpdateFromModel(model);

      eventAggregator.OnModelUpdate(model, UpdateFromModel);
    }

    private void AddLoop(Loop loop)
    {
      var newItem = loopSourceExportFactory.CreateExport().Value;
      newItem.SetModel(loop);
      entrySources[loop] = new WaveToSampleProvider(newItem);
      mixer.AddMixerInput(newItem);
    }

    private void UpdateFromModel(Ambience model)
    {
      var oldEntries = new HashSet<Ambience.Entry>(entrySources.Keys);
      var newEntries = new HashSet<Ambience.Entry>(model.Entries);

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

      foreach (var newEntry in newEntries.OfType<Loop>())
      {
        AddLoop(newEntry);  
      }
    }

    public ISampleProvider Audio => mixer;
    public int Read(byte[] buffer, int offset, int count)
    {
      return output.Read(buffer, offset, count);
    }

    public WaveFormat WaveFormat => output.WaveFormat;
  }
}