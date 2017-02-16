using System.ComponentModel.Composition;
using System.Windows.Input;
using AmbientOTron.Views.Shell;
using AmbientOTron.Views.SoundBoard;
using Core.Extensions;
using Core.Navigation;
using Core.Repository;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Ambience
{
  [Export]
  public class NewAmbienceViewModel : BindableBase
  {
    private readonly INavigationService navigationService;
    private readonly IRepository repository;

    [ImportingConstructor]
    public NewAmbienceViewModel(INavigationService navigationService, IRepository repository)
    {
      this.navigationService = navigationService;
      this.repository = repository;

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
      var newModel = new Core.Repository.Models.Ambience
      {
        Name = name,
      };

      repository.Add(newModel);

      navigationService.NavigateAsync<AmbienceView>(
        ShellViewModel.MainRegion,
        new NavigationParameters().WithModel(newModel));
    }
  }
}