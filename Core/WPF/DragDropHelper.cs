using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace Core.WPF
{
  public class DragDropHelper : ICollection<DragDropHelper.DragDropHelperEntry>, IDropTarget
  {
    private readonly List<DragDropHelperEntry> entries = new List<DragDropHelperEntry>();

    public class DragDropHelperEntry
    {
      public Func<IDropInfo, bool> Matches { get; set; }
      public Func<IDropInfo, DragDropEffects> Effects { get; set; }
      public Action<IDropInfo> ExecuteDrop { get; set; }
    }

    public void Add(Func<IDropInfo, bool> matches, Action<IDropInfo> executeDrop, Func<IDropInfo, DragDropEffects> effects = null)
    {
      ((ICollection<DragDropHelperEntry>)this).Add(new DragDropHelperEntry
      {
        Matches =  matches,
        Effects = effects ?? DefaultEffects,
        ExecuteDrop = executeDrop
      });
    }

    public DragDropEffects DefaultEffects(IDropInfo _)
    {
      return DragDropEffects.Copy;
    }

    #region Implementation of IEnumerable

    IEnumerator<DragDropHelperEntry> IEnumerable<DragDropHelperEntry>.GetEnumerator()
    {
      return entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) entries).GetEnumerator();
    }

    #endregion

    #region Implementation of ICollection<DragDropHelperEntry>

    void ICollection<DragDropHelperEntry>.Add(DragDropHelperEntry item)
    {
      entries.Add(item);
    }

    void ICollection<DragDropHelperEntry>.Clear()
    {
      entries.Clear();
    }

    bool ICollection<DragDropHelperEntry>.Contains(DragDropHelperEntry item)
    {
      return entries.Contains(item);
    }

    void ICollection<DragDropHelperEntry>.CopyTo(DragDropHelperEntry[] array, int arrayIndex)
    {
      entries.CopyTo(array, arrayIndex);
    }

    bool ICollection<DragDropHelperEntry>.Remove(DragDropHelperEntry item)
    {
      return entries.Remove(item);
    }

    int ICollection<DragDropHelperEntry>.Count
    {
      get { return entries.Count; }
    }

    bool ICollection<DragDropHelperEntry>.IsReadOnly
    {
      get { return ((ICollection<DragDropHelperEntry>) entries).IsReadOnly; }
    }

    #endregion

    #region Implementation of IDropTarget

    public void DragOver(IDropInfo dropInfo)
    {
      var matchingEntry = entries.FirstOrDefault(e => e.Matches(dropInfo));
      dropInfo.Effects = matchingEntry?.Effects(dropInfo) ?? DragDropEffects.None;
    }

    public void Drop(IDropInfo dropInfo)
    {
      var matchingEntry = entries.FirstOrDefault(e => e.Matches(dropInfo));
      matchingEntry?.ExecuteDrop(dropInfo);
    }

    #endregion

    public static bool IsFileDrop(IDropInfo dropInfo)
    {
      var dataObject = dropInfo.Data as DataObject;
        
      return (dataObject?.GetDataPresent(DataFormats.FileDrop) == true);
    }
  }
}
