using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Core.Audio;
using Core.Events;
using Core.Repository;
using Core.Repository.Models.Sources;
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

    [ImportingConstructor]
    public AudioSourceViewModel(IEventAggregator eventAggregator, IRepository repository, IAudioService audioService)
    {
      this.audioService = audioService;

      eventAggregator.GetEvent<UpdateModelEvent<AudioFile>>()
               .Subscribe(SetModel, ThreadOption.UIThread, false, m => m == Model);

      PlayCommand = new DelegateCommand(Play, () => !HasError).ObservesProperty(() => HasError);
    }

    public ICommand PlayCommand { get; }

    public AudioFile Model { get; set; }

    private async void Play()
    {
      try
      {
        await audioService.PlayAudioFile(Model);
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


    public void SetModel(AudioFile model)
    {
      Model = model;
      Name = model.Name;

      HasError = false;
    }
  }
}