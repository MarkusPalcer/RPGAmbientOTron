using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using AmbientOTron.Views.Ambience.Entries;
using Core.Events;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Ambience
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class AmbienceViewModel : BindableBase, INavigationAware, IAmbienceEntryVisitor, IDisposable
  {
    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();
    private readonly IEventAggregator eventAggregator;
    private readonly ExportFactory<LoopViewModel> loopViewModelFactory;
    private readonly IRepository repository;
    private readonly List<IDisposable> viewModelDisposables = new List<IDisposable>();
    private AmbienceEntryViewModel loopCreator;

    [ImportingConstructor]
    public AmbienceViewModel(IEventAggregator eventAggregator, ExportFactory<LoopViewModel> loopViewModelFactory, IRepository repository)
    {
      this.eventAggregator = eventAggregator;
      this.loopViewModelFactory = loopViewModelFactory;
      this.repository = repository;
    }

    public ObservableCollection<AmbienceEntryViewModel> Entries { get; } = new ObservableCollection<AmbienceEntryViewModel>();

    public string Name => Model.Name;

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      Model = navigationContext.GetModel<Core.Repository.Models.Ambience>();

      loopCreator = new NewLoopViewModel(Model, eventAggregator, repository);

      modelUpdateSubscription.Disposable = eventAggregator.OnModelUpdate(Model, UpdateFromModel);

      UpdateFromModel();
    }

    private void UpdateFromModel()
    {
      OnPropertyChanged(() => Name);

      Entries.Clear();
      viewModelDisposables.ForEach(x => x.Dispose());
      viewModelDisposables.Clear();

      foreach (var entry in Model.Entries)
      {
        entry.Accept(this);
      }

      Entries.Add(loopCreator);
    }

    public Core.Repository.Models.Ambience Model { get; set; }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext) {}

    #region Creation of entry view models
    public void Visit(Loop model)
    {
      var viewModelExport = loopViewModelFactory.CreateExport();
      viewModelExport.Value.SetModel(model);
      Entries.Add(viewModelExport.Value);
      viewModelDisposables.Add(viewModelExport);
    }
    #endregion

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
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

