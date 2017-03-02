using System.IO;
using System.Linq;
using System.Windows;
using AmbientOTron.Views.Ambience.Entries;
using Core.Events;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Events;

namespace AmbientOTron.Views.Ambience
{
  public class NewLoopViewModel : AmbienceEntryViewModel, IDropTarget
  {
    private readonly AmbienceModel model;
    private readonly IEventAggregator eventAggregator;
    private readonly IRepository repository;
    private readonly DragDropHelper dragDropHelper;

    public NewLoopViewModel(AmbienceModel model, IEventAggregator eventAggregator, IRepository repository)
    {
      this.model = model;
      this.eventAggregator = eventAggregator;
      this.repository = repository;

      dragDropHelper = new DragDropHelper
      {
        {DragDropHelper.IsFileDrop, AddFiles},
        {x => x.Data is LoopModel, CopyLoop},
        {x => x.Data is Core.Repository.Sounds.SoundModel, AddSound}
      };
    }

    private void AddSound(IDropInfo obj)
    {
      model.Entries.Add(new LoopModel
      {
        Sound = (obj.Data as Core.Repository.Sounds.SoundModel)?.Clone()
      });

      eventAggregator.ModelUpdated(model);
    }

    private void CopyLoop(IDropInfo obj)
    {
      model.Entries.Add((obj.Data as LoopModel)?.Clone());

      eventAggregator.ModelUpdated(model);
    }

    private async void AddFiles(IDropInfo dropInfo)
    {
      var newFiles = (string[])((DataObject)dropInfo.Data).GetData(DataFormats.FileDrop);
      if (newFiles == null)
        return;

      foreach (var file in newFiles)
      {
        if (!File.Exists(file))
          continue;

        var source = await repository.ImportFile(file);

        if (model.Entries.OfType<LoopModel>().Any(x => x.Sound == source))
          continue;

        model.Entries.Add(
          new LoopModel
          {
            Sound = source
          });
      }

      eventAggregator.ModelUpdated(model);
    }

    public void DragOver(IDropInfo dropInfo)
    {
      ((IDropTarget) dragDropHelper).DragOver(dropInfo);
    }

    public void Drop(IDropInfo dropInfo)
    {
      ((IDropTarget) dragDropHelper).Drop(dropInfo);
    }

    public override string Name { get; } = "Create loop";
  }
}