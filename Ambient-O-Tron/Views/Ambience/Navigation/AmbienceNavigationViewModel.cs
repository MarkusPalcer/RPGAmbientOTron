using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using AmbientOTron.Views.Dialogs.MessageBox;
using AmbientOTron.Views.Navigation;
using Core.Dialogs;
using Core.Extensions;
using Core.Navigation;
using Core.Repository.Models;
using Core.Util;
using Prism.Commands;
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
    private readonly IDialogService dialogService;
    private readonly DynamicVisitor<AmbienceModel.Entry> dynamicVisitor;


    [ImportingConstructor]
    public AmbienceNavigationViewModel(INavigationService navigationService, IEventAggregator eventAggregator, ExportFactory<LoopNavigationViewModel> loopViewModelFactory, IDialogService dialogService)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      this.dialogService = dialogService;

      dynamicVisitor = new DynamicVisitor<AmbienceModel.Entry>();
      dynamicVisitor.Register(CreateItemViewModelFactory<LoopModel, LoopNavigationViewModel,object>(loopViewModelFactory));

      ContextMenuEntries = new[] {
        new NavigationItemContextMenuEntry("Delete", new DelegateCommand(Delete))
      };
    }

    private async void Delete()
    {
      var choice = await dialogService.ShowMessageBox(
        $"Do you really want to delete the Ambience '{Model.Name}'?",
        MessageBoxButtons.YesNo,
        DialogResult.No);

      if (choice != DialogResult.Yes)
        return;

      eventAggregator.ModelRemoved(Model);
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