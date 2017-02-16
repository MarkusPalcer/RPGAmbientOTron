using System.IO;
using System.Linq;
using System.Windows;
using AmbientOTron.Views.Ambience.Entries;
using Core.Events;
using Core.Repository;
using Core.Repository.Models;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Events;

namespace AmbientOTron.Views.Ambience
{
  public class NewLoopViewModel : AmbienceEntryViewModel, IDropTarget
  {
    private readonly Core.Repository.Models.Ambience model;
    private readonly IEventAggregator eventAggregator;
    private readonly IRepository repository;
    private readonly DragDropHelper dragDropHelper;

    public NewLoopViewModel(Core.Repository.Models.Ambience model, IEventAggregator eventAggregator, IRepository repository)
    {
      this.model = model;
      this.eventAggregator = eventAggregator;
      this.repository = repository;

      dragDropHelper = new DragDropHelper
      {
        {DragDropHelper.IsFileDrop, AddFiles},
        {x => x.Data is Loop, CopyLoop},
        {x => x.Data is Core.Repository.Sounds.Sound, AddSound}
      };
    }

    private void AddSound(IDropInfo obj)
    {
      model.Entries.Add(new Loop
      {
        Sound = (obj.Data as Core.Repository.Sounds.Sound)?.Clone()
      });

      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.Ambience>>().Publish(model);
    }

    private void CopyLoop(IDropInfo obj)
    {
      model.Entries.Add((obj.Data as Loop)?.Clone());

      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.Ambience>>().Publish(model);
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

        if (model.Entries.OfType<Loop>().Any(x => x.Sound == source))
          continue;

        model.Entries.Add(
          new Loop
          {
            Sound = source
          });
      }

      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.Ambience>>().Publish(model);
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