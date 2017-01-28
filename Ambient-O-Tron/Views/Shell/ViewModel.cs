using System.ComponentModel.Composition;
using System.Windows.Input;
using AmbientOTron.Views.Gaming.SoundBoard;
using Core.Dialogs;
using Core.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Shell
{
    [Export]
    public class ViewModel : BindableBase
    {
        private readonly IDialogService dialogService;

        [ImportingConstructor]
        public ViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            LibraryEditorNavigationCommand = navigationService.CreateNavigationCommand(new NavigationRequestCollection
            {
                { ResourcesPane, typeof(Editors.LibraryEditor.MasterView) }
            });
      
        }

        public ICommand LibraryEditorNavigationCommand { get; }


      public const string ResourcesPane = "RessourcesPane";
      public const string MainRegion = "MainRegion";
      public const string LowerPane = "LowerPane";
      public const string PropertiesPane = "PropertiesPane";
    }
}
