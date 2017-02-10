using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Windows.Media;
using Core.Events;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardEntryPropertyViewModel : BindableBase, INavigationAware, IDisposable
  {
    private Core.Repository.Models.SoundBoard.Entry model;
    private readonly IEventAggregator eventAggregator;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    [ImportingConstructor]
    public SoundBoardEntryPropertyViewModel(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      model = (Core.Repository.Models.SoundBoard.Entry) navigationContext.Parameters["model"];
      disposables.Add(eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.SoundBoard.Entry>>()
                                     .Subscribe(_ => LoadFromModel(), ThreadOption.UIThread, true, m => m == model));
      LoadFromModel();
    }

    private void LoadFromModel()
    {
      name = model.Sound.Name;
      OnPropertyChanged(() => Name);

      color = model.Color;
      OnPropertyChanged(() => Color);
    }


    private string name = string.Empty;

    public string Name
    {
      get { return name; }
      set
      {
        if (SetProperty(ref name, value))
        {
          model.Sound.Name = value;
          eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.SoundBoard.Entry>>().Publish(model);
        }
      }
    }

    private Color color = Colors.Transparent;

    public Color Color
    {
      get { return color; }
      set
      {
        if (SetProperty(ref color, value))
        {
          model.Color = value;
          eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.SoundBoard.Entry>>().Publish(model);
        }
      }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return false;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

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
        disposables.Dispose();
      }

      // Release unmanaged resources here
    }
  }
}