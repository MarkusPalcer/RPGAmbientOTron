using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Core.Repository.Models;
using Prism.Events;

namespace AmbientOTron.Views.Ambience.Entries
{
  [Export]
  public class LoopViewModel : AmbienceEntryViewModel, IDisposable, IWithModel<Loop>
  {
    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();
    private readonly IEventAggregator eventAggregator;
    private Loop model;

    [ImportingConstructor]
    public LoopViewModel(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;
    }

    public override string Name => model.Name;

    public Loop Model
    {
      get { return model; }
      set
      {
        model = value;
        UpdateFromModel();

        modelUpdateSubscription.Disposable = eventAggregator.OnModelUpdate(model, UpdateFromModel);
      }
    }

    object IWithModel.Model => Model;

    private void UpdateFromModel()
    {
      OnPropertyChanged(() => Name);
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