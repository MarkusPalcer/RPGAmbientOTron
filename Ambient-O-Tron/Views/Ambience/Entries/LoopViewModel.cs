using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Windows.Input;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Properties;
using Core.Extensions;
using Core.Navigation;
using Core.Repository.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using static System.Single;

namespace AmbientOTron.Views.Ambience.Entries
{
  [Export]
  public class LoopViewModel : AmbienceEntryViewModel, IDisposable, IWithModel<LoopModel>
  {
    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();
    private readonly IEventAggregator eventAggregator;
    private LoopModel model;
    private readonly DelegateCommand togglePlaybackCommand;

    [ImportingConstructor]
    public LoopViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.eventAggregator = eventAggregator;

      togglePlaybackCommand = new DelegateCommand(
        () =>
        {
          model.IsPlaying = !model.IsPlaying;
          eventAggregator.ModelUpdated(model);
        },
        () => model != null);

      PropertyCommand = new DelegateCommand(() => navigationService.NavigateAsync<PropertiesView>(Shell.ShellViewModel.PropertiesPane, new NavigationParameters().WithModel(model)));
    }

    public ICommand PropertyCommand { get; }

    public ICommand TogglePlaybackCommand => togglePlaybackCommand;

    public override string Name => model.Name;

    public bool IsPlaying => model.IsPlaying;

    public float Volume
    {
      get { return Model.Volume; }
      set
      {
        if (Math.Abs(Model.Volume - value) < Epsilon)
          return;

        Model.Volume = value;

        eventAggregator.ModelUpdated(Model);
      }
    }

    public LoopModel Model
    {
      get { return model; }
      set
      {
        model = value;
        togglePlaybackCommand.RaiseCanExecuteChanged();
        UpdateFromModel();

        modelUpdateSubscription.Disposable = eventAggregator.OnModelUpdate(model, UpdateFromModel);
      }
    }

    object IWithModel.Model => Model;

    private void UpdateFromModel()
    {
      OnPropertyChanged(() => Name);
      OnPropertyChanged(() => Volume);
      OnPropertyChanged(() => IsPlaying);
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
        modelUpdateSubscription.Dispose();
      }

      // Release unmanaged resources here
    }

    
  }
}