using System.ComponentModel.Composition;
using AmbientOTron.Views.Shell;

namespace AmbientOTron
{
    [Export]
    public partial class MainWindow
    {
        [ImportingConstructor]
        public MainWindow(View view)
        {
            InitializeComponent();
            Content = view;
        }
    }
}
