using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Ambience
{
  [Export]
  public partial class NewAmbienceView
  {
    [ImportingConstructor]
    public NewAmbienceView(NewAmbienceViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
