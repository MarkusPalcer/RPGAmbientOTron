using System.ComponentModel.Composition;
using AmbientOTron.ViewModels;

namespace AmbientOTron.Views
{
    [Export]
    public partial class LibraryEditor 
    {
        [ImportingConstructor]
        public LibraryEditor(LibraryEditorViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}

