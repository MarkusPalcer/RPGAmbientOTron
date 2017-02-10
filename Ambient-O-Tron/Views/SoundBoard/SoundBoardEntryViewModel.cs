using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Core.Audio;
using Core.Events;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Sounds;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardEntryViewModel : BindableBase, IDisposable
  {
    private readonly IAudioService audioService;
    private readonly INavigationService navigationService;

    private readonly SerialDisposable statusSubscription = new SerialDisposable();
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    [ImportingConstructor]
    public SoundBoardEntryViewModel(IEventAggregator eventAggregator, IRepository repository, IAudioService audioService, INavigationService navigationService)
    {
      this.audioService = audioService;
      this.navigationService = navigationService;

      PlayCommand = new DelegateCommand(Play, () => !HasError).ObservesProperty(() => HasError);

      disposables.Add(statusSubscription);
      disposables.Add(eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.SoundBoard.Entry>>().Subscribe(_ => UpdateFromModel(), ThreadOption.UIThread, true, m => m == Model));
    }

    public ICommand PlayCommand { get; }
    private ICommand propertyCommand;

    public ICommand PropertyCommand
    {
      get { return propertyCommand; }
      private set { SetProperty(ref propertyCommand, value); }
    }

    public Core.Repository.Models.SoundBoard.Entry Model { get; set; }

    private async void Play()
    {
      try
      {
        await audioService.Play(Model.Sound);
      }
      catch (Exception)
      {
        HasError = true;
      }
    }

    public string Name => Model.Sound.Name;

    private bool hasError = true;

    public bool HasError
    {
      get { return hasError; }
      set { SetProperty(ref hasError, value); }
    }

    public Color Color => Model.Color;

    public void SetModel(Core.Repository.Models.SoundBoard.Entry model)
    {
      Model = model;
      statusSubscription.Disposable = model.Sound.Status.Select(x => x != Status.Ready).Subscribe(x => HasError = x);
      PropertyCommand =
  navigationService.CreateNavigationCommand<SoundBoardEntryPropertyView>(
    Shell.ShellViewModel.PropertiesPane,
    new NavigationParameters
    {
            {"model", Model}
    });
      UpdateFromModel();
    }

    private void UpdateFromModel()
    {
      OnPropertyChanged(() => Color);
      OnPropertyChanged(() => Name);
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