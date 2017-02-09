using System.ComponentModel.Composition;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Shell
{
    [Export]
    public class ShellViewModel : BindableBase
    {
        [ImportingConstructor]
        public ShellViewModel(IRegionManager regionManager)
        {
          ClosePropertiesCommand = new DelegateCommand(() => regionManager.Regions[PropertiesPane].RemoveAll());
        }

      public ICommand ClosePropertiesCommand { get; }

      public const string ResourcesPane = "RessourcesPane";
      public const string MainRegion = "MainRegion";
      public const string LowerPane = "LowerPane";
      public const string PropertiesPane = "PropertiesPane";
    }
}
