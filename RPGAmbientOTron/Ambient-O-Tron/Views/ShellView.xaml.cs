using System.ComponentModel.Composition;
using System.Windows.Controls;
using AmbientOTron.ViewModels;

namespace AmbientOTron.Views
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
