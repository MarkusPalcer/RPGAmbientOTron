using System.ComponentModel.Composition;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public partial class NewSoundBoardView 
  {
    [ImportingConstructor]
    public NewSoundBoardView(NewSoundBoardViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
