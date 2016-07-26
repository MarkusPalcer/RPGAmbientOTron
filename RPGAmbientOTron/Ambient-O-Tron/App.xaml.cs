using System;
using System.ComponentModel.Composition;
using System.Windows;
using AmbientOTron.ViewModels;
using AmbientOTron.Views;
using AmbientOTron.Views.Tabs;
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

            bootstrapper.Container.GetExportedValue<IRegionManager>()
                        .RegisterViewWithRegion(SoundEffectTabRegionName, typeof(SoundEffectView));

        }

    }
}
