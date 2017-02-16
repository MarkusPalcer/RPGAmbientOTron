using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Navigation;
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
    private readonly IEventAggregator eventAggregator;
    private readonly ExportFactory<LoopNavigationViewModel> loopViewModelFactory;

    [ImportingConstructor]
    public AmbienceNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator, ExportFactory<LoopNavigationViewModel> loopViewModelFactory)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      this.loopViewModelFactory = loopViewModelFactory;
    }

    protected override void OnModelSet(Core.Repository.Models.Ambience newModel)
    {
      NavigateCommand = navigationService.CreateNavigationCommand<AmbienceView>(
        Shell.ShellViewModel.MainRegion,
        new NavigationParameters().WithModel(Model));
      eventAggregator.OnModelUpdate(Model, UpdateFromModel);
    }

    protected override void UpdateFromModel()
    {
      Name = Model.Name;

      Items.Clear();

      Items.AddRange(
        Model.Entries.OfType<Loop>().Select(CreateLoopViewModel));
    }

    private LoopNavigationViewModel CreateLoopViewModel(Loop model)
    {
      var export = loopViewModelFactory.CreateExport();
      var result = export.Value;

      result.Model = model;
      return result;
    }
  }


  [Export]
  public class LoopNavigationViewModel : NavigationItemViewModel<Loop>
  {
    protected override void UpdateFromModel()
    {
      Name = Model.Name;
    }
  }
}