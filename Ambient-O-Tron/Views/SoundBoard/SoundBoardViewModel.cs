using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using AmbientOTron.Views.Gaming.SoundBoard;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Sounds;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Mvvm;
using Prism.Regions;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget, IDisposable
  {
    private readonly ExportFactory<SoundBoardEntryViewModel> audioFileViewModelFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly DragDropHelper dragDropHelper;
    private readonly IRepository repository;

    private ObservableCollection<SoundBoardEntryViewModel> files;
    private Core.Repository.Models.SoundBoard model;

    private string name = "Unnamed soundboard";

    [ImportingConstructor]
    public SoundBoardViewModel(
      IRepository repository,
      ExportFactory<SoundBoardEntryViewModel> audioFileViewModelFactory,
      INavigationService navigationService)
    {
      this.repository = repository;
      this.audioFileViewModelFactory = audioFileViewModelFactory;
      Files = new ObservableCollection<SoundBoardEntryViewModel>();

      dragDropHelper = new DragDropHelper
      {
        {DragDropHelper.IsFileDrop, DropFile},
        {dropInfo => dropInfo.Data is SoundBoardEntryViewModel, ReorderEntries, _ => DragDropEffects.Move},
        {dropInfo => dropInfo.Data is Sound, DropModel }
      };

      PropertiesCommand =
        navigationService.CreateNavigationCommand<SoundBoardPropertiesView>(
          Shell.ShellViewModel.PropertiesPane,
          new NavigationParameters
          {
            {"ViewModel", this}
          });
    }

    private void SaveChanges()
    {
      if (model == null)
      {
        model = new Core.Repository.Models.SoundBoard();
      }

      model.Name = Name;
      model.Entries = Files.Select(x => x.Model).ToList();
      repository.Save(model);
    }

    public ObservableCollection<SoundBoardEntryViewModel> Files
    {
      get { return files; }
      set { SetProperty(ref files, value); }
    }

    public string Name
    {
      get { return name; }
      set
      {
        if (SetProperty(ref name, value) && model.Name != value)
        {
          SaveChanges();
        }
      }
    }

    public ICommand PropertiesCommand { get; private set; }

    #region IDisposable

    public void Dispose()
    {
      disposables?.Dispose();
    }

    #endregion

    private void ReorderEntries(IDropInfo dropInfo)
    {
      Files.Move(Files.IndexOf(dropInfo.Data as SoundBoardEntryViewModel), dropInfo.InsertIndex);
      SaveChanges();
    }

    private async void DropFile(IDropInfo dropInfo)
    {
      var newFiles = (string[]) ((DataObject) dropInfo.Data).GetData(DataFormats.FileDrop);
      if (newFiles == null)
        return;

      foreach (var file in newFiles)
      {
        if (!File.Exists(file))
        {
          continue;
        }

        var source = await repository.ImportFile(file);

        if (Files.Any(x => x.Model.Sound == source))
          continue;

        Files.Add(CreateSourceViewModel(new Core.Repository.Models.SoundBoard.Entry()
        {
          Sound = source
        }));
      }

      SaveChanges();
    }

    private void DropModel(IDropInfo dropInfo)
    {
      var droppedModel = dropInfo.Data as Sound;
      if (droppedModel == null)
      {
        return;
      }

      Files.Add(CreateSourceViewModel(new Core.Repository.Models.SoundBoard.Entry()
      {
        Sound = droppedModel.Clone()
      }));

      SaveChanges();
    }

    private void LoadFromModel()
    {
      Name = model.Name;
      Files.Clear();
      Files.AddRange(model.Entries.Select(CreateSourceViewModel));
    }

    private SoundBoardEntryViewModel CreateSourceViewModel(Core.Repository.Models.SoundBoard.Entry forModel)
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
