using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Layout.MasterDetail
{
    [Export]
    public partial class View
    {
        [ImportingConstructor]
        public View(ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
