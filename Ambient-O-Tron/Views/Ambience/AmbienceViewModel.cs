using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Windows.Input;
using AmbientOTron.Views.Ambience.Entries;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Properties;
using Core.Extensions;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Models;
using Core.Util;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Ambience
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class AmbienceViewModel : BindableBase, INavigationAware, IDisposable
  {
    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();
    private readonly IEventAggregator eventAggregator;
    private readonly IRepository repository;
    private readonly List<IDisposable> viewModelDisposables = new List<IDisposable>();
    private AmbienceEntryViewModel loopCreator;
    private readonly DynamicVisitor<AmbienceModel.Entry> entryViewModelCreator = new DynamicVisitor<AmbienceModel.Entry>();
    private readonly DynamicVisitor<AmbienceEntryViewModel> entryDeleter = new DynamicVisitor<AmbienceEntryViewModel>();

    [ImportingConstructor]
    public AmbienceViewModel(IEventAggregator eventAggregator, ExportFactory<LoopViewModel> loopViewModelFactory, IRepository repository, INavigationService navigationService)
    {
      this.eventAggregator = eventAggregator;
      this.repository = repository;

      entryViewModelCreator.Register(CreateViewModelFactory<LoopModel,LoopViewModel>(loopViewModelFactory));

      entryDeleter.Register(CreateViewModelRemoval<LoopModel,LoopViewModel>());

      PropertiesCommand = new DelegateCommand(() =>
            navigationService.NavigateAsync<PropertiesView>(
              Shell.ShellViewModel.PropertiesPane,
              new NavigationParameters().WithModel(Model)));

      DeleteEntryCommand = new DelegateCommand<AmbienceEntryViewModel>(DeleteEntry);
    }

    private void DeleteEntry(AmbienceEntryViewModel entry)
    {
      entryDeleter.Visit(entry);
    }

    public ICommand DeleteEntryCommand { get; set; }

    private Action<TViewModel> CreateViewModelRemoval<TModel, TViewModel>()
      where TViewModel : AmbienceEntryViewModel, IWithModel<TModel>
      where TModel : AmbienceModel.Entry
    {
      return vm =>
      {
        Model.Entries.Remove(vm.Model);
        eventAggregator.ModelUpdated(Model);
      };
    }

    private Action<TModel> CreateViewModelFactory<TModel, TViewModel>(ExportFactory<TViewModel> exportFactory)
      where TViewModel: AmbienceEntryViewModel, IWithModel<TModel>
    {
      return m =>
      {
        var viewModelExport = exportFactory.CreateExport();
        viewModelExport.Value.Model = m;
        Entries.Add(viewModelExport.Value);
        viewModelDisposables.Add(viewModelExport);
      };
    }

    public ObservableCollection<AmbienceEntryViewModel> Entries { get; } = new ObservableCollection<AmbienceEntryViewModel>();

    public string Name => Model.Name;

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      Model = navigationContext.GetModel<AmbienceModel>();

      loopCreator = new NewLoopViewModel(Model, eventAggregator, repository);

      modelUpdateSubscription.Disposable = eventAggregator.OnModelUpdate(Model, UpdateFromModel);

      Model.IsPlaying = true;
      eventAggregator.ModelUpdated(Model);
    }

    private void UpdateFromModel()
    {
      OnPropertyChanged(() => Name);

      Entries.Clear();
      viewModelDisposables.ForEach(x => x.Dispose());
      viewModelDisposables.Clear();

      foreach (var entry in Model.Entries)
      {
        entryViewModelCreator.Visit(entry);
      }

      Entries.Add(loopCreator);
    }

    public AmbienceModel Model { get; set; }

    public ICommand PropertiesCommand { get; }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
      Model.IsPlaying = false;
      eventAggregator.ModelUpdated(Model);
    }


    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        Entries.Clear();
        viewModelDisposables.ForEach(x => x.Dispose());
        viewModelDisposables.Clear();
      }

      // Release unmanaged resources here
    }
  }
}

