using System.IO;
using System.Windows.Input;
using Core.Events;
using Core.Navigation;
using Core.Repository.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    public class LibraryViewModel : BindableBase
    {
        private string name;
        private string fileName;
        private string fullFileName;

        public LibraryViewModel(Library model, IEventAggregator eventAggregator, INavigationService navigationService)
        {
            SetModel(model);
            eventAggregator.GetEvent<UpdateModelEvent<Library>>()
                           .Subscribe(SetModel, ThreadOption.UIThread, false, x => x.Path == FullFileName);

            EditCommand =
                navigationService.CreateNavigationCommand<DetailView>(
                    Shell.ViewModel.DetailRegion,
                    new NavigationParameters
                    {
                        {"id", FullFileName}
                    });
        }

        public ICommand EditCommand { get; }

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

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
    }
}