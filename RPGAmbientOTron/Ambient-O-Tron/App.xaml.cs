using System.ComponentModel.Composition;
using System.Windows;
using NAudio.Wave;
using Prism.Regions;

namespace AmbientOTron
{
    public partial class App
    {
        private Bootstrapper bootstrapper;

        public const string MainRegionName = "MAIN";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            InitializeNAudio();
        }

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
