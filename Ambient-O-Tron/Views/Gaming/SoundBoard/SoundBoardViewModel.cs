using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using Core.Repository;
using Core.Repository.Models;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Mvvm;
using Prism.Regions;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace AmbientOTron.Views.Gaming.SoundBoard
{
  [Export]
  public class SoundBoardViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget, IDisposable
  {
    private readonly ExportFactory<AudioFileViewModel> audioFileViewModelFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly DragDropHelper dragDropHelper;
    private readonly IRepository repository;

    private ObservableCollection<AudioFileViewModel> files;
    private Core.Repository.Models.SoundBoard model;

    private string name = "Unnamed soundboard";

    [ImportingConstructor]
    public SoundBoardViewModel(
      IRepository repository,
      ExportFactory<AudioFileViewModel> audioFileViewModelFactory)
    {
      this.repository = repository;
      this.audioFileViewModelFactory = audioFileViewModelFactory;
      Files = new ObservableCollection<AudioFileViewModel>();

      dragDropHelper = new DragDropHelper
      {
        {DragDropHelper.IsFileDrop, DropFile},
        {dropInfo => dropInfo.Data is AudioFileViewModel, ReorderEntries, _ => DragDropEffects.Move}
      };
    }

    private void SaveChanges()
    {
      if (model == null)
      {
        model = new Core.Repository.Models.SoundBoard();
      }

      model.Name = Name;
      model.Sounds = Files.Select(x => x.Model).ToList();
      repository.Save(model);
    }

    public ObservableCollection<AudioFileViewModel> Files
    {
      get { return files; }
      set { SetProperty(ref files, value); }
    }

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    #region IDisposable

    public void Dispose()
    {
      disposables?.Dispose();
    }

    #endregion

    private void ReorderEntries(IDropInfo dropInfo)
    {
      Files.Move(Files.IndexOf(dropInfo.Data as AudioFileViewModel), dropInfo.InsertIndex);
      SaveChanges();
    }

    private void DropFile(IDropInfo dropInfo)
    {
      var newFiles = (string[]) ((DataObject) dropInfo.Data).GetData(DataFormats.FileDrop);
      if (newFiles == null)
        return;

      foreach (var file in newFiles)
      {
        if (Files.Any(x => x.Model.FullPath == file))
          continue;

        Files.Add(CreateFileViewModel(repository.GetAudioFileModel(file)));
      }

      SaveChanges();
    }

    private void LoadFromModel()
    {
      Name = model.Name;
      Files.Clear();
      Files.AddRange(model.Sounds.Select(CreateFileViewModel));
    }

    private AudioFileViewModel CreateFileViewModel(AudioFile forModel)
    {
      var export = audioFileViewModelFactory.CreateExport();
      disposables.Add(export);

      var result = export.Value;
      result.SetModel(forModel);
      return result;
    }

    #region Implementation of INavigationAware

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      var id = navigationContext.Parameters?["id"] as Guid? ?? Guid.Empty;
      model = repository.LoadSoundBoard(id) ?? new Core.Repository.Models.SoundBoard();

      LoadFromModel();
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext) {}

    public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
      continuationCallback(true);
    }

    #endregion

    #region Implementation of IDropTarget

    public void DragOver(IDropInfo dropInfo)
    {
      dragDropHelper.DragOver(dropInfo);
    }

    public void Drop(IDropInfo dropInfo)
    {
      dragDropHelper.Drop(dropInfo);
    }

    #endregion
  }
}
