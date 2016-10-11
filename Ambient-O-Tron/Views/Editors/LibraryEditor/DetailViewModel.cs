using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Core.Dialogs;
using Core.Navigation;
using Core.Persistence;
using Core.Repository;
using Core.Repository.Models;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Application = System.Windows.Application;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using AmbientOTron.Views.Dialogs.MessageBox;
using ViewModel = AmbientOTron.Views.Layout.MasterDetail.ViewModel;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public class DetailViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget
    {
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IRepository repository;

        private bool isDirty;
        private Library model;

        private string name;

        [ImportingConstructor]
        public DetailViewModel(
          INavigationService navigationService,
          IRepository repository,
          IEventAggregator eventAggregator,
          IDialogService dialogService)
        {
            this.navigationService = navigationService;
            this.repository = repository;
            this.dialogService = dialogService;
            Files.CollectionChanged += (sender, args) => IsDirty = true;

            RevertCommand = new DelegateCommand(LoadFromModel).ObservesCanExecute(p => IsDirty);
            CloseCommand = new DelegateCommand(CloseDetailView);
            SaveCommand = new DelegateCommand(SaveLibrary).ObservesCanExecute(p => IsDirty);
        }

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

        public ICommand SaveCommand { get; }

        private bool IsDirty
        {
            get { return isDirty; }
            set { SetProperty(ref isDirty, value); }
        }

        private void SaveLibrary()
        {
            model.Name = name;
            model.Files.Clear();
            model.Files.AddRange(Files.Select(x => x.Model));
            // TODO: Set Sattelite libraries

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

                if (dlg.ShowDialog(Application.Current.MainWindow) != true)
                {
                    return;
                }

                model.Path = dlg.FileName;
            }

            repository.Save(model);

            CloseDetailView();
        }
        
        private void LoadFromModel()
        {
            Name = model.Name;
            Files.Clear();
            Files.AddRange(model.Files.Select(CreateFileViewModel));
            IsDirty = false;
        }

        private FileViewModel CreateFileViewModel(AudioFile arg)
        {
            var result = new FileViewModel(arg);
            result.DeleteCommand = new DelegateCommand(() => Files.Remove(result));

            return result;
        }

        private void CloseDetailView()
        {
            IsDirty = false;
            navigationService.NavigateAsync<Empty>(ViewModel.DetailRegion);
        }

        #region IConfirmNavigationRequest

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var libraryId = navigationContext.Parameters?["id"] as string;
            model = libraryId == null 
                ? new Library() 
                : repository.GetLibraryModel(libraryId);

            LoadFromModel();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (IsDirty)
            {
                var result =
                    await
                    dialogService.ShowMessageBox(
                        "Do you want to leave this page and loose your changes?",
                        MessageBoxButtons.YesNo,
                        DialogResult.No);

                continuationCallback(result == DialogResult.Yes);
            }
            else
            {
                continuationCallback(true);
            }
        }

        #endregion

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
                var files = (string[])dataObject.GetData(DataFormats.FileDrop);
                Files.AddRange(files.Select(fileName =>
                {
                    var result = new FileViewModel(repository.GetAudioFileModel(fileName));
                    return result;
                }));
            }
        }

        #endregion
    }
}
