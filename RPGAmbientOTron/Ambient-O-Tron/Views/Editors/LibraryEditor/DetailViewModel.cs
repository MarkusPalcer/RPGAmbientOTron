using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Core.Repository.Models;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public class DetailViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget
    {
        private Library model = null;

        [ImportingConstructor]
        public DetailViewModel()
        {
            Files.CollectionChanged += (sender, args) => IsDirty = true;

            RevertCommand = new DelegateCommand(LoadFromModel).ObservesCanExecute(p => IsDirty);
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
                IsDirty = true;
            }
        }

        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();

        public ICommand RevertCommand { get; }

        private bool isDirty;

        private bool IsDirty
        {
            get { return isDirty; }
            set { SetProperty(ref isDirty, value); }
        }

        private void LoadFromModel()
        {
            Name = model.PersistenceModel.Name;
            Files.Clear();
            Files.AddRange(model.Files.Select(x => new FileViewModel(x)));
            IsDirty = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters?["id"] == null)
            {
                model = new Library
                {
                    PersistenceModel = new Core.Persistence.Models.Library()
                };

            }
            else
            {
                throw new NotImplementedException();
            }

            LoadFromModel();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            // TODO: When dirty, ask if user wants to lose changes
            continuationCallback(true);
        }

        public class FileViewModel : BindableBase
        {
            private AudioFile model;

            public FileViewModel(AudioFile model)
            {
                this.model = model;
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.None;

            var dataObject = dropInfo.Data as DataObject;

            if (dataObject?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                var files = (string[]) dataObject.GetData(DataFormats.FileDrop);
                Files.AddRange(files.Select(CreateFileViewModel));
            }
        }

        private FileViewModel CreateFileViewModel(string fileName)
        {
            var model = new AudioFile
            {
                PersistenceModel = new Core.Persistence.Models.AudioFile
                {
                    FileName = fileName,
                    Name = new FileInfo(fileName).Name
                }
            };

            return new FileViewModel(model);
        }
    }
}