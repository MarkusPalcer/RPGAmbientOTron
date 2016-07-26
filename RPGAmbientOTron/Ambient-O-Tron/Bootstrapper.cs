using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Windows;
using Prism.Mef;
using Prism.Regions;

namespace AmbientOTron
{
    public class Bootstrapper : MefBootstrapper
    {

        protected override AggregateCatalog CreateAggregateCatalog()
        {
            var catalog = base.CreateAggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));

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
            return Container.GetExportedValue<MainWindow>();
        }
    }
}