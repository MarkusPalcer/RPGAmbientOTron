using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Navigation;
using Prism.Events;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  public class SoundBoardNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.SoundBoard>
  {
    private readonly INavigationService navigationService;

    public SoundBoardNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.SoundBoard>>().Subscribe(_ => UpdateFromModel());
    }

    protected override void UpdateFromModel()
    {
      NavigateCommand = navigationService.CreateNavigationCommand<SoundBoardView>(
        Shell.ShellViewModel.LowerPane,
        new NavigationParameters
        {
                {"id", Model.Id}
        });

      Name = Model.Name;
    }
  }
}