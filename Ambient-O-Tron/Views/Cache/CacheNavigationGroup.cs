using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Repository;
using Prism.Events;

namespace AmbientOTron.Views.Cache
{
  [Export(typeof(NavigationGroup<>))]
  public class CacheNavigationGroup:NavigationGroup<CacheNavigationViewModel>
  {
    private readonly ExportFactory<CacheNavigationViewModel> itemFactory;

    [ImportingConstructor]
    public CacheNavigationGroup(IRepository repository, IEventAggregator eventAggregator, ExportFactory<CacheNavigationViewModel> itemFactory)
    {
      this.itemFactory = itemFactory;
      Name = "Caches";
      var items = new ObservableCollection<CacheNavigationViewModel>(repository.GetCaches().Select(CreateItemViewModel));

      eventAggregator.GetEvent<AddModelEvent<Core.Repository.Models.Cache>>()
                     .Subscribe(x => items.Add(CreateItemViewModel(x)), ThreadOption.UIThread, true);

      Items = items;
    }

    private CacheNavigationViewModel CreateItemViewModel(Core.Repository.Models.Cache model)
    {
      var result = itemFactory.CreateExport().Value;
      result.Model = model;
      return result;
    }
  }
}