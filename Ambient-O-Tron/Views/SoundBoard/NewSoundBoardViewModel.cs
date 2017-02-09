﻿using System.ComponentModel.Composition;
using System.Windows.Input;
using AmbientOTron.Views.Gaming.SoundBoard;
using AmbientOTron.Views.Shell;
using Core.Navigation;
using Core.Repository;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class NewSoundBoardViewModel : BindableBase
  {
    private readonly INavigationService navigationService;
    private readonly IRepository repository;

    [ImportingConstructor]
    public NewSoundBoardViewModel(INavigationService navigationService, IRepository repository)
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
      var newModel = new Core.Repository.Models.SoundBoard
      {
        Name = name,
      };

      repository.Save(newModel);
      navigationService.NavigateAsync<SoundBoardView>(
        ViewModel.LowerPane,
        new NavigationParameters
        {
          {"id", newModel.Id}
        });
    }
  }
}