using System;
using System.IO;
using System.Windows.Input;
using Core.Repository.Models;
using Prism.Mvvm;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    public class FileViewModel : BindableBase
    {
        private string name;

        // TODO: Rename
        public FileViewModel(AudioFile model)
        {
            Model = model;
            Name = model.Name;
            FullFileName = model.FullPath;
        }

        public AudioFile Model { get; }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string FileName => new FileInfo(FullFileName).Name;

        public string FullFileName { get; }

        public bool HasError => Model.LoadStatus != LoadStatus.FileOk;

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
    }
}