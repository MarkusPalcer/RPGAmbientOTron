using System.ComponentModel.Composition;

namespace AmbientOTron.Views.Properties
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public partial class PropertiesView
  {
    [ImportingConstructor]
    public PropertiesView(PropertiesViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
    }
  }
}
