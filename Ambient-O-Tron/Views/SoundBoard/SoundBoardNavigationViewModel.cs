using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Core.Navigation;
using Prism.Events;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  public class SoundBoardNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.SoundBoard>
  {
    private readonly INavigationService navigationService;

    public SoundBoardNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator, Core.Repository.Models.SoundBoard model)
    {
      this.navigationService = navigationService;
      Model = model;
      eventAggregator.OnModelUpdate(model, UpdateFromModel);
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