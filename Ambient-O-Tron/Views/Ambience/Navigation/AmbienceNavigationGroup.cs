using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using AmbientOTron.Views.Ambience.Navigation;
using AmbientOTron.Views.Navigation;
using Core.Events;
using Prism.Events;

namespace AmbientOTron.Views.Ambience
{
  [Export(typeof(NavigationGroup<>))]
  public class AmbienceNavigationGroup : NavigationGroup<AmbienceNavigationViewModel>
  {
    private readonly ExportFactory<AmbienceNavigationViewModel> entryViewModelFactory;

    [ImportingConstructor]
    public AmbienceNavigationGroup(IEventAggregator eventAggregator, ExportFactory<AmbienceNavigationViewModel> entryViewModelFactory)
    {
      this.entryViewModelFactory = entryViewModelFactory;

      Name = "Ambiences";

      Items = new ObservableCollection<AmbienceNavigationViewModel>();
      eventAggregator.GetEvent<AddModelEvent<Core.Repository.Models.Ambience>>()
               .Subscribe(x => Items.Add(CreateItemViewModel(x)), ThreadOption.UIThread, true);
    }

    private AmbienceNavigationViewModel CreateItemViewModel(Core.Repository.Models.Ambience ambience)
    {
      var export = entryViewModelFactory.CreateExport();
      export.Value.Model = ambience;
      return export.Value;
    }
  }
}