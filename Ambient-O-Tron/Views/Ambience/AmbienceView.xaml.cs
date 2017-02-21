using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Ambience
{
  [Export]
  public partial class AmbienceView
  {
    [ImportingConstructor]
    public AmbienceView(AmbienceViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
