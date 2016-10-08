using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Core.Events;
using Core.Extensions;
using Core.Navigation;
using Core.Persistence;
using Core.Repository;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Library = Core.Repository.Models.Library;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public class MasterViewModel : IDropTarget
    {
        private readonly IRepository repository;
        private readonly INavigationService navigationService;
        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public MasterViewModel(IRepository repository, INavigationService navigationService, IEventAggregator eventAggregator)
        {
            this.repository = repository;
            this.navigationService = navigationService;
            this.eventAggregator = eventAggregator;


            Libraries = repository.Libraries
            .Select(x => new LibraryViewModel(x, eventAggregator))
            .ToObservableCollection();

            // No parameters -> Blank entry
            AddCommand = navigationService.CreateNavigationCommand<DetailView>(Layout.MasterDetail.ViewModel.DetailRegion);
            EditCommand = new DelegateCommand<LibraryViewModel>(ExecuteEditCommand);

            // TODO: Update list on edit
            eventAggregator.GetEvent<AddModelEvent<Library>>().Subscribe(HandleAddModel);
        }

        private void HandleAddModel(Library model)
        {
            Libraries.Add(new LibraryViewModel(model, eventAggregator));
        }

        private void ExecuteEditCommand(LibraryViewModel vm)
        {
            navigationService.NavigateAsync<DetailView>(
                                                        Layout.MasterDetail.ViewModel.DetailRegion,
                                                        new NavigationParameters
                                                        {
                                                            {"id", vm.FullFileName}
                                                        });
        }

        public ObservableCollection<LibraryViewModel> Libraries { get; }

        public ICommand AddCommand { get; }

        public ICommand EditCommand { get; }

        public class LibraryViewModel : BindableBase
        {
            private string name;
            private string fileName;
            private string fullFileName;

            // TODO: Delete, Edit (bold)

            public LibraryViewModel(Library model, IEventAggregator eventAggregator)
            {
                SetModel(model);
                eventAggregator.GetEvent<UpdateModelEvent<Library>>()
                               .Subscribe(SetModel, ThreadOption.UIThread, false, x => x.Path == FullFileName);
            }

            private void SetModel(Library model)
            {
                Name = model.Name;
                FullFileName = model.Path;
                FileName = new FileInfo(FullFileName).Name;
            }

            public string Name
            {
                get { return name; }
                set { SetProperty(ref name, value); }
            }

            public string FileName
            {
                get { return fileName; }
                set { SetProperty(ref fileName, value); }
            }

            public string FullFileName
            {
                get { return fullFileName; }
                set { SetProperty(ref fullFileName, value); }
            }
        }

        #region Implementation of IDropTarget

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.None;

            var dataObject = dropInfo.Data as DataObject;

            if (dataObject?.GetDataPresent(DataFormats.FileDrop) != true)
            {
                return;
            }

            var files = dataObject.GetData(DataFormats.FileDrop) as string[];
            if (files == null)
            {
                return;
            }

            if (files.All(x => x.EndsWith($".{Constants.LibraryExtension}"))) {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject?.GetDataPresent(DataFormats.FileDrop) != true)
            {
                return;
            }

            var files = (string[])dataObject.GetData(DataFormats.FileDrop);
            files.ForEach(repository.LoadLibrary);

            Libraries.Clear();
            Libraries.AddRange(repository.Libraries.Select(x => new LibraryViewModel(x, eventAggregator)));
        }

        #endregion
    }
}