using System.ComponentModel.Composition;
using System.Windows;
using NAudio.Wave;

namespace AmbientOTron
{
    public partial class App
    {
        private Bootstrapper bootstrapper;

        public const string MainRegionName = "MAIN";

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

            InitializeNAudio();

            
        }

        #region Overrides of Application

        protected override void OnExit(ExitEventArgs e)
        {
            bootstrapper.Container.Dispose();
            base.OnExit(e);
        }

        #endregion

        private void InitializeNAudio()
        {
            var mixer = new WaveMixerStream32
            {
                AutoStop = false
            };

            var waveOutDevice = new WaveOut();
            waveOutDevice.Init(mixer);
            waveOutDevice.Play();

            bootstrapper.Container.ComposeExportedValue(waveOutDevice);
            bootstrapper.Container.ComposeExportedValue(mixer);

        }
    }
}
