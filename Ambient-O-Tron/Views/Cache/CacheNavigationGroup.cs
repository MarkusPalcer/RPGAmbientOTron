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

    [ImportingConstructor]
    public CacheNavigationGroup(IRepository repository, IEventAggregator eventAggregator)
    {
      Name = "Caches";
      var items = new ObservableCollection<CacheNavigationViewModel>(repository.GetCaches().Select(CreateItemViewModel));

      eventAggregator.GetEvent<AddModelEvent<Core.Repository.Models.Cache>>()
                     .Subscribe(x => items.Add(CreateItemViewModel(x)), ThreadOption.UIThread, true);

      Items = items;
    }

    private CacheNavigationViewModel CreateItemViewModel(Core.Repository.Models.Cache model)
    {
      var result = new CacheNavigationViewModel();
      result.SetModel(model);
      return result;
    }
  }
}