using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Navigation;
using Core.Repository;
using Prism.Events;

namespace AmbientOTron.Views.Gaming.SoundBoard
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
                     .Subscribe(newModel => items.Add(CreateItemViewModel(newModel)), ThreadOption.UIThread);

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