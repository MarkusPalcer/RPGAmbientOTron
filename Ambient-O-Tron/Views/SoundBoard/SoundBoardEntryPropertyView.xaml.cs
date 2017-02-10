using System.ComponentModel.Composition;
using System.Windows;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public partial class SoundBoardEntryPropertyView 
  {
    [ImportingConstructor]
    public SoundBoardEntryPropertyView(SoundBoardEntryPropertyViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
