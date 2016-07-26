
using System.ComponentModel.Composition;
using AmbientOTron.ViewModels;
using AmbientOTron.ViewModels.Tabs;

namespace AmbientOTron.Views.Tabs
{
    [Export]
    public partial class SoundEffectView 
    {
        [ImportingConstructor]
        public SoundEffectView(SoundEffectsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
