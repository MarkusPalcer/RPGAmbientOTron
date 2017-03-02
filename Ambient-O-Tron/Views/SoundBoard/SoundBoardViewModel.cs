using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using AmbientOTron.Views.Properties;
using AmbientOTron.Views.Shell;
using Core.Events;
using Core.Extensions;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Sounds;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget, IDisposable
  {
    private readonly ExportFactory<SoundBoardEntryViewModel> audioFileViewModelFactory;
    private readonly SerialDisposable updateSubscription = new SerialDisposable();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly DragDropHelper dragDropHelper;
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private readonly IRepository repository;

    private ObservableCollection<SoundBoardEntryViewModel> files;
    private Core.Repository.Models.SoundBoardModel model;

    private string name = "Unnamed soundboard";

    [ImportingConstructor]
    public SoundBoardViewModel(
      IRepository repository,
      ExportFactory<SoundBoardEntryViewModel> audioFileViewModelFactory,
      INavigationService navigationService,
      IEventAggregator eventAggregator)
    {
      this.repository = repository;
      this.audioFileViewModelFactory = audioFileViewModelFactory;
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      Files = new ObservableCollection<SoundBoardEntryViewModel>();

      dragDropHelper = new DragDropHelper
      {
        {DragDropHelper.IsFileDrop, DropFile},
        {dropInfo => dropInfo.Data is SoundBoardEntryViewModel, ReorderEntries, _ => DragDropEffects.Move},
        {dropInfo => dropInfo.Data is SoundModel, DropModel}
      };

      disposables.Add(updateSubscription);
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
          SaveChanges();
      }
    }

    public ICommand PropertiesCommand { get; private set; }

    #region IDisposable

    public void Dispose()
    {
      disposables?.Dispose();
    }

    #endregion

    private void SaveChanges()
    {
      if (model == null)
        model = new Core.Repository.Models.SoundBoardModel();

      model.Name = Name;
      model.Entries = Files.Select(x => x.Model).ToList();
      eventAggregator.ModelUpdated(model);
    }

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
          continue;

        var source = await repository.ImportFile(file);

        if (Files.Any(x => x.Model.Sound == source))
          continue;

        Files.Add(
          CreateSourceViewModel(
            new Core.Repository.Models.SoundBoardModel.Entry
            {
              Sound = source
            }));
      }

      SaveChanges();
    }

    private void DropModel(IDropInfo dropInfo)
    {
      var droppedModel = dropInfo.Data as SoundModel;
      if (droppedModel == null)
        return;

      Files.Add(
        CreateSourceViewModel(
          new Core.Repository.Models.SoundBoardModel.Entry
          {
            Sound = droppedModel.Clone()
          }));

      SaveChanges();
    }

    private void UpdateFromModel()
    {
      Name = model.Name;
      Files.Clear();
      Files.AddRange(model.Entries.Select(CreateSourceViewModel));
    }

    private SoundBoardEntryViewModel CreateSourceViewModel(Core.Repository.Models.SoundBoardModel.Entry forModel)
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
      model = navigationContext.GetModel<Core.Repository.Models.SoundBoardModel>();

      PropertiesCommand = navigationService.CreateNavigationCommand<PropertiesView>(
        ShellViewModel.PropertiesPane,
        new NavigationParameters().WithModel(model));

      updateSubscription.Disposable = eventAggregator.OnModelUpdate(model, UpdateFromModel);

      UpdateFromModel();

      eventAggregator.GetEvent<InitializeSoundBoardEvent>().Publish(model);
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
