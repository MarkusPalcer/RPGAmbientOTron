using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Core.Audio;
using Core.Repository;
using Core.Repository.Sounds;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardEntryViewModel : BindableBase
  {
    private readonly IAudioService audioService;
    private string name;

    private readonly SerialDisposable statusSubscription = new SerialDisposable();

    [ImportingConstructor]
    public SoundBoardEntryViewModel(IEventAggregator eventAggregator, IRepository repository, IAudioService audioService)
    {
      this.audioService = audioService;

      PlayCommand = new DelegateCommand(Play, () => !HasError).ObservesProperty(() => HasError);
    }

    public ICommand PlayCommand { get; }

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

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

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
      Name = model.Sound.Name;
      statusSubscription.Disposable = model.Sound.Status.Select(x => x != Status.Ready).Subscribe(x => HasError = x);
    }
  }
}