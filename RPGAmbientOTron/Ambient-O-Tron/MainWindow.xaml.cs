using System.ComponentModel.Composition;
using AmbientOTron.Views;

namespace AmbientOTron
{
    [Export]
    public partial class MainWindow
    {
        [ImportingConstructor]
        public MainWindow(ShellView view)
        {
            InitializeComponent();
            Content = view;
        }
    }
}
