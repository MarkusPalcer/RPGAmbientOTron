using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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
            .Select(x => new LibraryViewModel(x, eventAggregator, navigationService))
            .ToObservableCollection();

            AddCommand = navigationService.CreateNavigationCommand<DetailView>(Shell.ViewModel.DetailRegion);

            eventAggregator.GetEvent<AddModelEvent<Library>>().Subscribe(HandleAddModel);
        }

        public ICommand AddCommand { get; }

        private void HandleAddModel(Library model)
        {
            Libraries.Add(new LibraryViewModel(model, eventAggregator, navigationService));
        }

        public ObservableCollection<LibraryViewModel> Libraries { get; }

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
        }

        #endregion
    }
}