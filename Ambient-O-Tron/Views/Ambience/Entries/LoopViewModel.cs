using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Windows.Input;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Core.Repository.Models;
using Prism.Commands;
using Prism.Events;

namespace AmbientOTron.Views.Ambience.Entries
{
  [Export]
  public class LoopViewModel : AmbienceEntryViewModel, IDisposable, IWithModel<Loop>
  {
    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();
    private readonly IEventAggregator eventAggregator;
    private Loop model;
    private readonly DelegateCommand togglePlaybackCommand;

    [ImportingConstructor]
    public LoopViewModel(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;

      togglePlaybackCommand = new DelegateCommand(
        () =>
        {
          model.IsPlaying = !model.IsPlaying;
          eventAggregator.ModelUpdated(model);
        },
        () => model != null);
    }

    public ICommand TogglePlaybackCommand => togglePlaybackCommand;

    public override string Name => model.Name;

    public bool IsPlaying => model.IsPlaying;

    public Loop Model
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