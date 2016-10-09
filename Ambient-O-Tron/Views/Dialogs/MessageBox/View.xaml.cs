using System.ComponentModel.Composition;
using Core.Dialogs;

namespace AmbientOTron.Views.Dialogs.MessageBox
{
[Export]
    public partial class View : IDialogView<int?>
    {
        [ImportingConstructor]
        public View(ViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

    #region Implementation of IDialogView<int?>

    public IDialogViewModel<int?> ViewModel => DataContext as IDialogViewModel<int?>;

    #endregion
    }
}
