using System;
using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Core.Audio;
using Core.Events;
using Core.Repository;
using Core.Repository.Sounds;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AmbientOTron.Views.Gaming.SoundBoard
{
  [Export]
  public class AudioSourceViewModel : BindableBase
  {
    private readonly IAudioService audioService;
    private string name;

    private readonly SerialDisposable statusSubscription = new SerialDisposable();

    [ImportingConstructor]
    public AudioSourceViewModel(IEventAggregator eventAggregator, IRepository repository, IAudioService audioService)
    {
      this.audioService = audioService;

      PlayCommand = new DelegateCommand(Play, () => !HasError).ObservesProperty(() => HasError);
    }

    public ICommand PlayCommand { get; }

    public Sound Model { get; set; }

    private async void Play()
    {
      try
      {
        await audioService.Play(Model);
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

    public void SetModel(Sound model)
    {
      Model = model;
      Name = model.Name;
      statusSubscription.Disposable = model.Status.Select(x => x != Status.Ready).Subscribe(x => HasError = x);
    }
  }
}