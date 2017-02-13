using System.ComponentModel.Composition;
using System.Reactive.Disposables;
using AmbientOTron.Views.Properties;
using AmbientOTron.Views.Properties.PropertyViewModels;
using Prism.Events;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardEntryPropertyViewModel : PropertiesViewModel
  {
    [ImportingConstructor]
    public SoundBoardEntryPropertyViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
    {
    }

    protected override void InitializeProperties()
    {
      var model = (Core.Repository.Models.SoundBoard.Entry)Model;

      Properties = new PropertyViewModel[]
      {
        new StringPropertyViewModel(() => model.Sound.Name, x => model.Sound.Name = x)
        {
          Name = "Name"
        }, 
        new ColorPropertyViewModel(() => model.Color, x => model.Color = x)
        {
          Name="Color"
        }, 
      };
    }

  }
}