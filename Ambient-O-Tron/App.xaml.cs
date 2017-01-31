using System.ComponentModel.Composition;
using System.Windows;
using AmbientOTron.Views.Gaming.SoundBoard;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Shell;
using NAudio.Wave;
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

        bootstrapper.Container.GetExportedValue<IRegionManager>().RegisterViewWithRegion(ViewModel.LowerPane, typeof(SoundBoardView));
        bootstrapper.Container.GetExportedValue<IRegionManager>().RegisterViewWithRegion(ViewModel.ResourcesPane, typeof(NavigationView));
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
