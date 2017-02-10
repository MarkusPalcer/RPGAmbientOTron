using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
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
    private readonly IRepository repository;
    private readonly IEventAggregator eventAggregator;

    [ImportingConstructor]
    public SoundBoardNavigationGroup(INavigationService navigationService, IRepository repository, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.repository = repository;
      this.eventAggregator = eventAggregator;

      Name = "SoundBoards";

      Items = new ObservableCollection<SoundBoardNavigationViewModel>();

      InitAsync();
    }

    private async void InitAsync()
    {
      Items.AddRange((await repository.GetSoundBoards()).Select(CreateItemViewModel));
      eventAggregator.GetEvent<AddModelEvent<Core.Repository.Models.SoundBoard>>()
                     .Subscribe(x => Items.Add(CreateItemViewModel(x)), ThreadOption.UIThread, true);
    }

    private SoundBoardNavigationViewModel CreateItemViewModel(Core.Repository.Models.SoundBoard model)
    {
      return new SoundBoardNavigationViewModel(navigationService, eventAggregator)
      {
        Model = model
      };
    }
  }
}