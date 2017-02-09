using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Shell
{
  [Export]
  public partial class ShellView
  {
    [ImportingConstructor]
    public ShellView(ShellViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
