using System.ComponentModel.Composition;
using System.Windows;
using NAudio.Wave;

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
