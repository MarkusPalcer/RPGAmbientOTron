using System;
using System.IO;
using System.Windows.Input;
using Core.Events;
using Core.Repository.Models;
using Prism.Events;
using Prism.Mvvm;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
  public class FileViewModel : BindableBase
  {
    private readonly IEventAggregator eventAggregator;
    private string name;

    // TODO: Rename
    public FileViewModel(AudioFile model, IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;

      eventAggregator.GetEvent<UpdateModelEvent<AudioFile>>()
                     .Subscribe(SetModel, ThreadOption.UIThread, false, m => m.FullPath == Model.FullPath);

      SetModel(model);
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

    public ICommand DeleteCommand { get; set; }

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
