using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Prism.Events;

namespace AmbientOTron.Views.SoundBoard.Navigation
{
  [Export(typeof(NavigationGroup<>))]
  public class SoundBoardNavigationGroup : NavigationGroup<SoundBoardNavigationViewModel>
  {
    private readonly ExportFactory<SoundBoardNavigationViewModel> itemFactory;

    [ImportingConstructor]
    public SoundBoardNavigationGroup(IEventAggregator eventAggregator, ExportFactory<SoundBoardNavigationViewModel> itemFactory)
    {
      this.itemFactory = itemFactory;

      Name = "SoundBoards";

      Items = new ObservableCollection<SoundBoardNavigationViewModel>();

      eventAggregator.OnModelAdd<Core.Repository.Models.SoundBoardModel>(
        x => Items.Add(CreateItemViewModel(x)));
    }

    private SoundBoardNavigationViewModel CreateItemViewModel(Core.Repository.Models.SoundBoardModel model)
    {
      var export = itemFactory.CreateExport();
      export.Value.Model = model;
      return export.Value;
    }
  }
}