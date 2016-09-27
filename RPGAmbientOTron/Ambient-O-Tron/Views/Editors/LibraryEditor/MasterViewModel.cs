using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Core.Extensions;
using Core.Navigation;
using Core.Persistence;
using Core.Persistence.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public class MasterViewModel
    {
        private readonly IPersistenceService persistenceService;
        private readonly INavigationService navigationService;

        [ImportingConstructor]
        public MasterViewModel(IPersistenceService persistenceService, INavigationService navigationService)
        {
            this.persistenceService = persistenceService;
            this.navigationService = navigationService;

            Libraries = persistenceService.Libraries
                .Select(x => new LibraryViewModel(x))
                .ToObservableCollection();

            // No parameters -> Blank entry
            AddCommand = navigationService.CreateNavigationCommand<DetailView>(Layout.MasterDetail.ViewModel.DetailRegion);
        }

        public ObservableCollection<LibraryViewModel> Libraries { get; }

        public ICommand AddCommand { get; }

        public class LibraryViewModel : BindableBase
        {
            private string name;
            private string fileName;
            private string fullFileName;

            public LibraryViewModel(Library model)
            {
                Name = model.Name;
                FullFileName = model.FileName;
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
    }
}