using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Navigation;
using Prism.Events;
using Prism.Regions;

namespace AmbientOTron.Views.Gaming.SoundBoard
{
  public class SoundBoardNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.SoundBoard>
  {
    private readonly INavigationService navigationService;

    public SoundBoardNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.SoundBoard>>().Subscribe(_ => UpdateFromModel());
    }

    private void UpdateFromModel()
    {
      Name = Model.Name;
    }

    public override void SetModel(Core.Repository.Models.SoundBoard newModel)
    {
      NavigateCommand = navigationService.CreateNavigationCommand<SoundBoardView>(
        Shell.ViewModel.LowerPane,
        new NavigationParameters
        {
          {"id", newModel.Id}
        });

      Model = newModel;

      UpdateFromModel();
    }
  }
}