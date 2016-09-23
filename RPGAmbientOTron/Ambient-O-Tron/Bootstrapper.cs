using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Core.Navigation;
using Prism.Mef;

namespace AmbientOTron
{
    public class Bootstrapper : MefBootstrapper
    {

        protected override AggregateCatalog CreateAggregateCatalog()
        {
            var catalog = base.CreateAggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(INavigationService).Assembly));

            return catalog;
        }

        public new CompositionContainer Container => base.Container;

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
#if DEBUGMEF
            return new Window
            {
                Title="MEF Debug view",
                WindowState = WindowState.Maximized,
                Content = new  MefContrib.Tools.Visualizer.Views.InformationView
                {
                    ViewModel = {
                        Container = Container
                    }
                }
            };
#else
            return Container.GetExportedValue<MainWindow>();
#endif
        }
    }
}