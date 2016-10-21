using System.ComponentModel.Composition;
using System.Windows.Input;
using Core.Dialogs;
using Core.Navigation;
using Prism.Commands;
using Prism.Mvvm;

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
                { MasterRegion, typeof(Editors.LibraryEditor.MasterView) }
            });
        }

        public ICommand LibraryEditorNavigationCommand { get; }

      public const string MasterRegion = "MasterDetail_Master";
      public const string DetailRegion = "MasterDetail_Detail";
  }
}
