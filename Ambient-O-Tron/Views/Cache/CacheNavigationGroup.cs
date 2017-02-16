using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Extensions;
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

      eventAggregator.OnModelAdd<Core.Repository.Models.Cache>(
        x => items.Add(CreateItemViewModel(x)));

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