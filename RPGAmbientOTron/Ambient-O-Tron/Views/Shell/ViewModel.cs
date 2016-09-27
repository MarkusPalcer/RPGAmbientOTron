using System.ComponentModel.Composition;
using System.Windows.Input;
using Core.Navigation;
using Prism.Mvvm;

namespace AmbientOTron.Views.Shell
{
    [Export]
    public class ViewModel : BindableBase
    {
        [ImportingConstructor]
        public ViewModel(INavigationService navigationService)
        {
            LibraryEditorNavigationCommand = navigationService.CreateNavigationCommand(new NavigationRequestCollection
            {
                { App.MainRegionName, typeof(Layout.MasterDetail.View) },
                { Layout.MasterDetail.ViewModel.MasterRegion, typeof(Editors.LibraryEditor.MasterView) }
            });
        }

        public ICommand LibraryEditorNavigationCommand { get; }
    }
}
