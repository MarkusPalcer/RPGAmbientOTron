using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Core.Repository;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Mvvm;

namespace AmbientOTron.Views.Navigation
{
  [Export]
  public class NavigationViewModel : BindableBase, IDragSource, IDropTarget
  {
    private readonly IRepository repository;
    private DragDropHelper dragDropHelper = new DragDropHelper();

    [ImportingConstructor]
    public NavigationViewModel([ImportMany(typeof(NavigationGroup<>))] IEnumerable<object> groups, IRepository repository)
    {
      this.repository = repository;

      Groups = new ObservableCollection<object>(groups);

      dragDropHelper.Add(DragDropHelper.IsFolderDrop, DropFolder);
    }

    private async void DropFolder(IDropInfo dropInfo)
    {
      var newFiles = (string[])((DataObject)dropInfo.Data).GetData(DataFormats.FileDrop);
      if (newFiles == null)
        return;

      foreach (var file in newFiles)
      {
        if (!Directory.Exists(file))
        {
          continue;
        }

        await repository.ImportCache(file);
      }
    }

    public ObservableCollection<object> Groups { get; }

    public void StartDrag(IDragInfo dragInfo)
    {
      var modelItem = dragInfo.SourceItem as IWithModel;
      dragInfo.Data = modelItem?.Model;
      if (dragInfo.Data != null)
      {
        dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Link;
      }
    }

    public bool CanStartDrag(IDragInfo dragInfo)
    {
      var modelItem = dragInfo.SourceItem as IWithModel;
      return modelItem != null;
    }

    public void Dropped(IDropInfo dropInfo)
    {
    }

    public void DragCancelled()
    {
    }

    public bool TryCatchOccurredException(Exception exception)
    {
      return false;
    }

    public void DragOver(IDropInfo dropInfo)
    {
      ((IDropTarget) dragDropHelper).DragOver(dropInfo);
    }

    public void Drop(IDropInfo dropInfo)
    {
      ((IDropTarget) dragDropHelper).Drop(dropInfo);
    }
  }
}