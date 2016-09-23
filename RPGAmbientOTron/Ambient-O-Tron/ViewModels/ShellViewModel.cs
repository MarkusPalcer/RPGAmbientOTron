using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using AmbientOTron.Views;
using Core.Navigation;
using Prism.Mvvm;

namespace AmbientOTron.ViewModels
{
    [Export]
    public class ShellViewModel : BindableBase
    {
        [ImportingConstructor]
        public ShellViewModel(INavigationService navigationService)
        {
            LibraryEditorNavigationCommand = navigationService.CreateNavigationCommand<LibraryEditor>(App.MainRegionName);
        }

        public ICommand LibraryEditorNavigationCommand { get; }
    }
}
