using System.ComponentModel.Composition;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Shell;
using Core.Extensions;
using Core.Navigation;
using Prism.Events;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard.Navigation
{
  [Export]
  public class SoundBoardNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.SoundBoard>
  {
    private readonly IEventAggregator eventAggregator;
    private readonly INavigationService navigationService;

    [ImportingConstructor]
    public SoundBoardNavigationViewModel(
      INavigationService navigationService,
      IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
    }

    protected override void OnModelSet(Core.Repository.Models.SoundBoard newModel)
    {
      NavigateCommand = navigationService.CreateNavigationCommand<SoundBoardView>(
        ShellViewModel.LowerPane,
        new NavigationParameters().WithModel(newModel));
      eventAggregator.OnModelUpdate(newModel, UpdateFromModel);
    }

    protected override void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}
