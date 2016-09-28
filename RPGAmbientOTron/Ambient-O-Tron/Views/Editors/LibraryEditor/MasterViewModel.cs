using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Core.Extensions;
using Core.Navigation;
using Core.Repository;
using Prism.Mvvm;
using Library = Core.Repository.Models.Library;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public class MasterViewModel
    {
        [ImportingConstructor]
        public MasterViewModel(IRepository repository, INavigationService navigationService)
        {
            Libraries = repository.Libraries
                .Select(x => new LibraryViewModel(x))
                .ToObservableCollection();

            // No parameters -> Blank entry
            AddCommand = navigationService.CreateNavigationCommand<DetailView>(Layout.MasterDetail.ViewModel.DetailRegion);

            // TODO: Update list on add or edit
        }

        public ObservableCollection<LibraryViewModel> Libraries { get; }

        public ICommand AddCommand { get; }

        public class LibraryViewModel : BindableBase
        {
            private string name;
            private string fileName;
            private string fullFileName;

            // TODO: Delete, Edit (bold)

            public LibraryViewModel(Library model)
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
    }
}