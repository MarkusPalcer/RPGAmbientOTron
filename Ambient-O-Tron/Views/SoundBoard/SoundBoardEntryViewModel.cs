using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using AmbientOTron.Views.Properties;
using AmbientOTron.Views.Shell;
using Core.Audio;
using Core.Events;
using Core.Extensions;
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
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly INavigationService navigationService;

    private readonly SerialDisposable statusSubscription = new SerialDisposable();

    private bool hasError = true;
    private ICommand propertyCommand;

    [ImportingConstructor]
    public SoundBoardEntryViewModel(
      IEventAggregator eventAggregator,
      IRepository repository,
      IAudioService audioService,
      INavigationService navigationService)
    {
      this.audioService = audioService;
      this.navigationService = navigationService;

      PlayCommand = new DelegateCommand(Play, () => !HasError).ObservesProperty(() => HasError);

      disposables.Add(statusSubscription);
      disposables.Add(eventAggregator.OnModelUpdate(Model, UpdateFromModel));
    }

    public ICommand PlayCommand { get; }

    public ICommand PropertyCommand
    {
      get { return propertyCommand; }
      private set { SetProperty(ref propertyCommand, value); }
    }

    public Core.Repository.Models.SoundBoard.Entry Model { get; set; }

    public string Name => Model.Sound.Name;

    public bool HasError
    {
      get { return hasError; }
      set { SetProperty(ref hasError, value); }
    }

    public Color Color => Model.Color;

    /// <summary>
    ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

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

    public void SetModel(Core.Repository.Models.SoundBoard.Entry model)
    {
      Model = model;
      statusSubscription.Disposable = model.Sound.Status.Select(x => x != Status.Ready).Subscribe(x => HasError = x);
      PropertyCommand = navigationService.CreateNavigationCommand<PropertiesView>(
        ShellViewModel.PropertiesPane,
        new NavigationParameters().WithModel(Model));
      UpdateFromModel();
    }

    private void UpdateFromModel()
    {
      OnPropertyChanged(() => Color);
      OnPropertyChanged(() => Name);
    }

    /// <summary>
    ///   Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        disposables.Dispose();

      // Release unmanaged resources here
    }
  }
}
