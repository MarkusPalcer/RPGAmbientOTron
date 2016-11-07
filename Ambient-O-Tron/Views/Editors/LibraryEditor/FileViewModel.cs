using System;
using System.IO;
using System.Reactive;
using System.Windows.Input;
using Core.Audio;
using Core.Events;
using Core.Repository;
using Core.Repository.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
  public class FileViewModel : BindableBase
  {
    private readonly IEventAggregator eventAggregator;
    private readonly IRepository repository;
    private readonly IAudioService audioService;
    private string name;

    public FileViewModel(AudioFile model, IEventAggregator eventAggregator, IRepository repository, IAudioService audioService)
    {
      this.eventAggregator = eventAggregator;
      this.repository = repository;
      this.audioService = audioService;

      eventAggregator.GetEvent<UpdateModelEvent<AudioFile>>()
                     .Subscribe(SetModel, ThreadOption.UIThread, false, m => m.FullPath == Model.FullPath);

      StartRenamingCommand = new DelegateCommand(StartRename);
      AcceptRenameCommand = new DelegateCommand(AcceptRename);
      CancelRenameCommand = new DelegateCommand(CancelRename);
      PreviewCommand = new DelegateCommand(StartPreview, () => !IsInPreview).ObservesProperty(() => IsInPreview);
      StopPreviewCommand = new DelegateCommand(() => Playback.Stop()).ObservesCanExecute(_ => IsInPreview);

      SetModel(model);
    }

    private async void StartPreview()
    {
      Playback = audioService.PlayAudioFile(Model.FullPath);
      Playback.Progress.Subscribe(p => Progress = p);
      await Playback;
      Playback = null;
    }

    private void AcceptRename()
    {
      Model.Name = Name;
      repository.Save(Model);
      IsInEditMode = false;
    }

    private void CancelRename()
    {
      Name = Model.Name;
      IsInEditMode = false;
    }

    private void StartRename()
    {
      IsInEditMode = true;
    }

    public AudioFile Model { get; set; }

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    public string FileName => new FileInfo(FullFileName).Name;

    private string fullFileName;

    public string FullFileName
    {
      get { return fullFileName; }
      set
      {
        SetProperty(ref fullFileName, value);
        OnPropertyChanged(() => FileName);
      }
    }

    public bool HasError => Model.LoadStatus != LoadStatus.FileOk;

    public LoadStatus Status => Model.LoadStatus;

    public bool IsInPreview => Playback != null;

    public string InfoText
    {
      get
      {
        switch (Model.LoadStatus)
        {
          case LoadStatus.Unknown:
            return "Unkown file status";
          case LoadStatus.FileNotFound:
            return "File not found";
          case LoadStatus.LoadError:
            return "Unrecognized file format";
          case LoadStatus.FileOk:
            return "Format: MP3";
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    private bool isInEditMode;
    private IPlayback playback;

    private IPlayback Playback
    {
      get { return playback; }
      set
      {
        SetProperty(ref playback, value);
        OnPropertyChanged(() => IsInPreview);
      }
    }

    private double progress = double.NaN;

    public double Progress
    {
      get { return progress; }
      private set { SetProperty(ref progress, value); }
    }

    public bool IsInEditMode
    {
      get { return isInEditMode; }
      set { SetProperty(ref isInEditMode, value); }
    }

    public ICommand DeleteCommand { get; set; }
    public ICommand AcceptRenameCommand { get; }
    public ICommand CancelRenameCommand { get;  }
    public ICommand StartRenamingCommand { get;  }
    public ICommand PreviewCommand { get; }
    public ICommand StopPreviewCommand { get; }

    private void SetModel(AudioFile model)
    {
      Model = model;
      Name = model.Name;
      FullFileName = model.FullPath;

      OnPropertyChanged(() => HasError);
      OnPropertyChanged(() => Status);
      OnPropertyChanged(() => InfoText);
    }
  }
}
