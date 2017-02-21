using System.ComponentModel.Composition;
using System.Windows.Input;
using AmbientOTron.Views.Shell;
using Core.Extensions;
using Core.Navigation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Ambience
{
  [Export]
  public class NewAmbienceViewModel : BindableBase
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;

    [ImportingConstructor]
    public NewAmbienceViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;

      SaveCommand = new DelegateCommand(Save, () => !string.IsNullOrEmpty(Name)).ObservesProperty(() => Name);
    }

    private string name;

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    public ICommand SaveCommand { get; set; }

    private void Save()
    {
      var newModel = new Core.Repository.Models.AmbienceModel
      {
        Name = name,
      };

      eventAggregator.ModelAdded(newModel);

      navigationService.NavigateAsync<AmbienceView>(
        ShellViewModel.MainRegion,
        new NavigationParameters().WithModel(newModel));
    }
  }
}