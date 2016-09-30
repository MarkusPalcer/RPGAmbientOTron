using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Core.Navigation;
using Core.Persistence;
using Core.Repository;
using Core.Repository.Models;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public class DetailViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget
    {
        private readonly INavigationService navigationService;
        private readonly IRepository repository;
        private readonly IEventAggregator eventAggregator;
        private Library model;

        [ImportingConstructor]
        public DetailViewModel(INavigationService navigationService, IRepository repository, IEventAggregator eventAggregator)
        {
            this.navigationService = navigationService;
            this.repository = repository;
            this.eventAggregator = eventAggregator;
            Files.CollectionChanged += (sender, args) => IsDirty = true;

            RevertCommand = new DelegateCommand(LoadFromModel).ObservesCanExecute(p => IsDirty);
            CloseCommand = new DelegateCommand(CloseDetailView);
            AddFileCommand = new DelegateCommand(ShowOpenFileDialog);
            SaveCommand = new DelegateCommand(SaveLibrary).ObservesCanExecute(p => IsDirty);
        }

        private void SaveLibrary()
        {
            if (string.IsNullOrEmpty(model.Path))
            {
                var dlg = new SaveFileDialog
                {
                    AddExtension = true,
                    DefaultExt = Constants.LibraryExtension,
                    Filter = $"Libraries|*.{Constants.LibraryExtension}",
                    OverwritePrompt = true,
                    Title = "Save library",
                };

                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                model.Path = dlg.FileName;
            }

            repository.Save(model);

            CloseDetailView();
        }

        private void ShowOpenFileDialog()
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
                Title = "Add file(s)...",
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            Files.AddRange(ofd.FileNames.Select(CreateFileViewModel));
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

        public ICommand CloseCommand { get; }

        public ICommand AddFileCommand { get; }
        public ICommand SaveCommand { get; }

        private bool isDirty;

        private bool IsDirty
        {
            get { return isDirty; }
            set { SetProperty(ref isDirty, value); }
        }

        private void LoadFromModel()
        {
            Name = model.Name;
            Files.Clear();
            Files.AddRange(model.Files.Select(x => new FileViewModel(x)));
            IsDirty = false;
        }

        private void CloseDetailView()
        {
            IsDirty = false;
            navigationService.NavigateAsync<Empty>(Layout.MasterDetail.ViewModel.DetailRegion);
        }

        #region IConfirmNavigationRequest
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters?["id"] == null)
            {
                model = new Library();
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
            if (IsDirty)
            {
                // TODO: Create and use DialogService
                var result = MessageBox.Show(
                                             "Do you want to leave this page and loose your changes?",
                                             "Leave",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning,
                                             MessageBoxResult.No);

                continuationCallback(result == MessageBoxResult.Yes);
            }
            else
            {
                continuationCallback(true);
            }
        }
        #endregion

        public class FileViewModel : BindableBase
        {
            
            // TODO: Rename
            public FileViewModel(AudioFile model)
            {
                Name = model.Name;
                FileName = model.FullPath;
            }

            private string name;

            public string Name
            {
                get { return name; }
                set { SetProperty(ref name, value); }
            }

            public string FileName { get; }

            public ICommand DeleteCommand { get; set; }
        }

        #region IDropTarget
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
        #endregion

        private FileViewModel CreateFileViewModel(string fileName)
        {
            var result = new FileViewModel(repository.GetAudioFileModel(fileName));
            result.DeleteCommand = new DelegateCommand(() => Files.Remove(result));
            return result;
        }
    }
}