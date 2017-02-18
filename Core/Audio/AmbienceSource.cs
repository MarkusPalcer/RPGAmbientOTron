using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Extensions;
using Core.Repository.Models;
using NAudio.Wave;
using Prism.Events;

namespace Core.Audio
{
  [Export]
  public class AmbienceSource : IAudioSource
  {
    private readonly ExportFactory<LoopSource> loopSourceExportFactory;
    private readonly IEventAggregator eventAggregator;
    private readonly WaveMixerStream32 mixer;

    private readonly Dictionary<Ambience.Entry, IAudioSource> entrySources = new Dictionary<Ambience.Entry, IAudioSource>();

    [ImportingConstructor]
    public AmbienceSource(ExportFactory<LoopSource> loopSourceExportFactory, IEventAggregator eventAggregator)
    {
      this.loopSourceExportFactory = loopSourceExportFactory;
      this.eventAggregator = eventAggregator;
      mixer = new WaveMixerStream32
      {
        AutoStop = false,
      };
    }

    public void SetModel(Ambience model)
    {
      foreach (var loop in model.Entries.OfType<Loop>())
      {
        AddLoop(loop);
      }

      eventAggregator.OnModelUpdate(model, UpdateFromModel);
    }

    private void AddLoop(Loop loop)
    {
      var newItem = loopSourceExportFactory.CreateExport().Value;
      newItem.SetModel(loop);
      entrySources[loop] = newItem;
      mixer.AddInputStream(newItem.Audio);
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
        mixer.RemoveInputStream(item.Audio);
      }

      foreach (var newEntry in newEntries.OfType<Loop>())
      {
        AddLoop(newEntry);  
      }
    }

    public WaveStream Audio => mixer;
  }
}