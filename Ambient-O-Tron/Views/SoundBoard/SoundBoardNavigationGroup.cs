using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Gaming.SoundBoard;
using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Navigation;
using Core.Repository;
using Prism.Events;

namespace AmbientOTron.Views.SoundBoard
{
  [Export(typeof(NavigationGroup<>))]
  public class SoundBoardNavigationGroup : NavigationGroup<SoundBoardNavigationViewModel>
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;

    [ImportingConstructor]
    public SoundBoardNavigationGroup(INavigationService navigationService, IRepository repository, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;

      Name = "SoundBoards";
      var items = new ObservableCollection<SoundBoardNavigationViewModel>(repository.GetSoundBoards().Select(CreateItemViewModel));

      eventAggregator.GetEvent<AddModelEvent<Core.Repository.Models.SoundBoard>>()
                     .Subscribe(x => items.Add(CreateItemViewModel(x)), ThreadOption.UIThread, true);

      Items = items;
    }

    private SoundBoardNavigationViewModel CreateItemViewModel(Core.Repository.Models.SoundBoard model)
    {
      var result = new SoundBoardNavigationViewModel(navigationService, eventAggregator);
      result.SetModel(model);
      return result;
    }
  }
}