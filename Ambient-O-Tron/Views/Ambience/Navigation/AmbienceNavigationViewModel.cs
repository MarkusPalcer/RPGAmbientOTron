using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Navigation;
using Core.Events;
using Core.Extensions;
using Core.Navigation;
using Core.Repository.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Ambience.Navigation
{
   [Export]
  public class AmbienceNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.Ambience, BindableBase>
  {
    private readonly INavigationService navigationService;

    [ImportingConstructor]
    public AmbienceNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.Ambience>>()
                     .Subscribe(_ => UpdateFromModel(), ThreadOption.UIThread, true, m => ReferenceEquals(m, Model));
    }

    protected override void UpdateFromModel()
    {
      NavigateCommand = navigationService.CreateNavigationCommand<AmbienceView>(
        Shell.ShellViewModel.MainRegion,
        new NavigationParameters().WithModel(Model));

      Name = Model.Name;

      Items.Clear();

      Items.AddRange(
        Model.Entries.OfType<Loop>().Select(
          m => new LoopNavigationViewModel
          {
            Model = m
          }));
    }
  }

  public class LoopNavigationViewModel : NavigationItemViewModel<Loop>
  {
    protected override void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}