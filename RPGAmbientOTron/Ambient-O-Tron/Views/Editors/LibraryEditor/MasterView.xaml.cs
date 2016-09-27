

using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Editors.LibraryEditor
{
    [Export]
    public partial class MasterView
    {
        [ImportingConstructor]
        public MasterView(MasterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
