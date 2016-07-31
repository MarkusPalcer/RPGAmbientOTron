using System.ComponentModel.Composition;
using System.Windows;
using AmbientOTron.Views.Tabs;
using NAudio.Wave;
using Prism.Regions;

namespace AmbientOTron
{
    public partial class App
    {
        public static string SoundEffectTabRegionName = "SoundEffectTab";

        private Bootstrapper bootstrapper;



        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            InitializeNAudio();

            bootstrapper.Container.GetExportedValue<IRegionManager>()
                        .RegisterViewWithRegion(SoundEffectTabRegionName, typeof(SoundEffectView));
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
