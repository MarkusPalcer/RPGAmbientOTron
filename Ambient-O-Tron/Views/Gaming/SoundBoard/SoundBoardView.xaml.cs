using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Gaming.SoundBoard
{
  [Export]
  public partial class SoundBoardView 
  {
    [ImportingConstructor]
    public SoundBoardView(SoundBoardViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
