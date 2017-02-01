using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Models.Sources;
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
    private readonly ExportFactory<AudioSourceViewModel> audioFileViewModelFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly DragDropHelper dragDropHelper;
    private readonly IRepository repository;

    private ObservableCollection<AudioSourceViewModel> files;
    private Core.Repository.Models.SoundBoard model;

    private string name = "Unnamed soundboard";

    [ImportingConstructor]
    public SoundBoardViewModel(
      IRepository repository,
      ExportFactory<AudioSourceViewModel> audioFileViewModelFactory,
      INavigationService navigationService)
    {
      this.repository = repository;
      this.audioFileViewModelFactory = audioFileViewModelFactory;
      Files = new ObservableCollection<AudioSourceViewModel>();

      dragDropHelper = new DragDropHelper
      {
        {DragDropHelper.IsFileDrop, DropFile},
        {dropInfo => dropInfo.Data is AudioSourceViewModel, ReorderEntries, _ => DragDropEffects.Move}
      };

      PropertiesCommand =
        navigationService.CreateNavigationCommand<SoundBoardPropertiesView>(
          Shell.ViewModel.PropertiesPane,
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
      model.Sounds = Files.Select(x => x.Model).ToList();
      repository.Save(model);
    }

    public ObservableCollection<AudioSourceViewModel> Files
    {
      get { return files; }
      set { SetProperty(ref files, value); }
    }

    public string Name
    {
      get { return name; }
      set
      {
        if (SetProperty(ref name, value))
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
      Files.Move(Files.IndexOf(dropInfo.Data as AudioSourceViewModel), dropInfo.InsertIndex);
      SaveChanges();
    }

    private void DropFile(IDropInfo dropInfo)
    {
      var newFiles = (string[]) ((DataObject) dropInfo.Data).GetData(DataFormats.FileDrop);
      if (newFiles == null)
        return;

      foreach (var file in newFiles)
      {
        var source = repository.GetSource(file);

        if (Files.Any(x => x.Model == source))
          continue;

        Files.Add(CreateSourceViewModel(source));
      }

      SaveChanges();
    }

    private void LoadFromModel()
    {
      Name = model.Name;
      Files.Clear();
      Files.AddRange(model.Sounds.Select(CreateSourceViewModel));
    }

    private AudioSourceViewModel CreateSourceViewModel(AudioFile forModel)
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
