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
            FileName = model.FullPath;
        }

        public AudioFile Model { get; }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string FileName { get; }
    }
}