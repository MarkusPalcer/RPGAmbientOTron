using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using Prism.Mvvm;

namespace AmbientOTron.Views.Navigation
{
  [Export]
  public class NavigationViewModel : BindableBase, IDragSource
  {
    [ImportingConstructor]
    public NavigationViewModel([ImportMany(typeof(NavigationGroup<>))] IEnumerable<object> groups)
    {
      Groups = new ObservableCollection<object>(groups);
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
  }
}