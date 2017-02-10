using System.Windows;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Shell;
using AmbientOTron.Views.SoundBoard;
using Prism.Regions;

namespace AmbientOTron
{
    public partial class App
    {
        private Bootstrapper bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var options = new CommandLineOptions();
            CommandLine.Parser.Default.ParseArguments(e.Args, options);

            bootstrapper = new Bootstrapper
            {
                MefDebugger = options.DebugMef
            };

            bootstrapper.Run();

        bootstrapper.Container.GetExportedValue<IRegionManager>().RegisterViewWithRegion(ShellViewModel.LowerPane, typeof(NewSoundBoardView));
        bootstrapper.Container.GetExportedValue<IRegionManager>().RegisterViewWithRegion(ShellViewModel.ResourcesPane, typeof(NavigationView));
    }

        #region Overrides of Application

        protected override void OnExit(ExitEventArgs e)
        {
            bootstrapper.Container.Dispose();
            base.OnExit(e);
        }

        #endregion
    }
}
