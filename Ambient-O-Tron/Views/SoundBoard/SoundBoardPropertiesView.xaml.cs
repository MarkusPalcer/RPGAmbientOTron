using System.ComponentModel.Composition;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public partial class SoundBoardPropertiesView
  {
    [ImportingConstructor]
    public SoundBoardPropertiesView(SoundBoardPropertiesViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
