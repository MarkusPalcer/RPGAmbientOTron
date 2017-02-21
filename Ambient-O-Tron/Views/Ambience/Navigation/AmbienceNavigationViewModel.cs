using System;
using System.ComponentModel.Composition;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Core.Navigation;
using Core.Repository.Models;
using Core.Util;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Ambience.Navigation
{
   [Export]
  public class AmbienceNavigationViewModel : NavigationItemViewModel<AmbienceModel, BindableBase>
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private readonly DynamicVisitor<AmbienceModel.Entry> dynamicVisitor;


    [ImportingConstructor]
    public AmbienceNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator, ExportFactory<LoopNavigationViewModel> loopViewModelFactory)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;

      dynamicVisitor = new DynamicVisitor<AmbienceModel.Entry>();
      dynamicVisitor.Register(CreateItemViewModelFactory<Loop, LoopNavigationViewModel,object>(loopViewModelFactory));
    }

    protected override void OnModelSet(AmbienceModel newModel)
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

      Model.Entries.ForEach(dynamicVisitor.Visit);
    }

    private Action<TModel> CreateItemViewModelFactory<TModel, TViewModel, TChildren>(ExportFactory<TViewModel> factory)
      where TViewModel:NavigationItemViewModel<TModel, TChildren> 
      where TModel : class
    {
      return m =>
      {
        var export = factory.CreateExport();
        var result = export.Value;

        result.Model = m;
        Items.Add(result);
      };
    }
  }
}