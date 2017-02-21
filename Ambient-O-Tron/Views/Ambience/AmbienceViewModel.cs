﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using AmbientOTron.Views.Ambience.Entries;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Models;
using Core.Util;
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
    private readonly ExportFactory<LoopViewModel> loopViewModelFactory;
    private readonly IRepository repository;
    private readonly List<IDisposable> viewModelDisposables = new List<IDisposable>();
    private AmbienceEntryViewModel loopCreator;
    private readonly DynamicVisitor<AmbienceModel.Entry> entryViewModelCreator = new DynamicVisitor<AmbienceModel.Entry>();

    [ImportingConstructor]
    public AmbienceViewModel(IEventAggregator eventAggregator, ExportFactory<LoopViewModel> loopViewModelFactory, IRepository repository)
    {
      this.eventAggregator = eventAggregator;
      this.loopViewModelFactory = loopViewModelFactory;
      this.repository = repository;

      entryViewModelCreator.Register(CreateViewModelFactory<Loop,LoopViewModel>(loopViewModelFactory));
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

