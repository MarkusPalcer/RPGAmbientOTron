using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Shell
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
