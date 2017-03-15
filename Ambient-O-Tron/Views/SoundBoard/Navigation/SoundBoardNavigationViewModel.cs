using System.ComponentModel.Composition;
using System.Windows.Forms;
using AmbientOTron.Views.Dialogs.MessageBox;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Shell;
using Core.Dialogs;
using Core.Extensions;
using Core.Navigation;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard.Navigation
{
  [Export]
  public class SoundBoardNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.SoundBoardModel>
  {
    private readonly IEventAggregator eventAggregator;
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;

    [ImportingConstructor]
    public SoundBoardNavigationViewModel(
      INavigationService navigationService,
      IEventAggregator eventAggregator,
      IDialogService dialogService)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      this.dialogService = dialogService;

      ContextMenuEntries = new[] {
        new NavigationItemContextMenuEntry("Delete", new DelegateCommand(Delete))
      };
    }

    private async void Delete()
    {
      var choice = await dialogService.ShowMessageBox(
        $"Do you really want to delete the Soundboard '{Model.Name}'?",
        MessageBoxButtons.YesNo,
        DialogResult.No);

      if (choice != DialogResult.Yes)
        return;

      eventAggregator.ModelRemoved(Model);
    }

    protected override void OnModelSet(Core.Repository.Models.SoundBoardModel newModel)
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
