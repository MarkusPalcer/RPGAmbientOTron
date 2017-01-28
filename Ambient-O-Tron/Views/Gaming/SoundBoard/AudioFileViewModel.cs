using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Core.Audio;
using Core.Events;
using Core.Repository;
using Core.Repository.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AmbientOTron.Views.Gaming.SoundBoard
{
  [Export]
  public class AudioFileViewModel : BindableBase
  {
    private readonly IAudioService audioService;
    private string name;

    [ImportingConstructor]
    public AudioFileViewModel(IEventAggregator eventAggregator, IRepository repository, IAudioService audioService)
    {
      this.audioService = audioService;

      eventAggregator.GetEvent<UpdateModelEvent<AudioFile>>()
               .Subscribe(SetModel, ThreadOption.UIThread, false, m => m.FullPath == Model.FullPath);

      PlayCommand = new DelegateCommand(Play, () => !HasError).ObservesProperty(() => HasError);
    }

    public ICommand PlayCommand { get; }

    public AudioFile Model { get; set; }

    private async void Play()
    {
      try
      {
        await audioService.PlayAudioFile(Model.FullPath);
      }
      catch (Exception)
      {
        Model.LoadStatus = LoadStatus.LoadError;
        OnPropertyChanged(() => HasError);
      }
    }

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    public bool HasError => Model.LoadStatus != LoadStatus.FileOk;


    public void SetModel(AudioFile model)
    {
      Model = model;
      Name = model.Name;

      OnPropertyChanged(() => HasError);
    }
  }
}