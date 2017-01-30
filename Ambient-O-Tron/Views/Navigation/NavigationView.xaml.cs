using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Navigation
{
  [Export]
  public partial class NavigationView
  {
    [ImportingConstructor]
    public NavigationView(NavigationViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
