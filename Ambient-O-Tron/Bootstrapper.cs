using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Core.Navigation;
using Prism.Mef;

namespace AmbientOTron
{
    public class Bootstrapper : MefBootstrapper
    {

        public bool MefDebugger { get; set; } = false;

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
            return MefDebugger ? CreateMefDebugWindow() : CreateMainWindow();
        }

        private Window CreateMainWindow()
        {
            try
            {
                return Container.GetExportedValue<MainWindow>();
            }
            catch (Exception)
            {
                return CreateMefDebugWindow();
            }
        }

        public Window CreateMefDebugWindow()
        {
            return new Window
            {
                Title = "MEF Debug window",
                WindowState = WindowState.Maximized,
                Content = new MefContrib.Tools.Visualizer.Views.InformationView
                {
                    ViewModel = {
                        Container = Container
                    }
                }
            };
        }
    }
}