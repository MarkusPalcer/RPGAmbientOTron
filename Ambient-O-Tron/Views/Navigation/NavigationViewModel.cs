using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using AmbientOTron.Views.Gaming.SoundBoard;
using Core.Events;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Navigation
{
  [Export]
  public class NavigationViewModel : BindableBase
  {
    [ImportingConstructor]
    public NavigationViewModel(IRepository repository, IEventAggregator eventAggregator, INavigationService navigationService)
    {
      Groups = new ObservableCollection<object>
      {
        new ItemGroup<SoundBoard, Guid>(
          "Sound boards",
          repository.GetSoundBoards(),
          m => m.Name,
          m => m.Id,
          eventAggregator,
          m => navigationService.NavigateAsync<SoundBoardView>(
            Shell.ViewModel.LowerPane,
            new NavigationParameters
            {
              {"id", m.Id}
            }))
      };

    }

    public class ItemGroup<TModel, TId>
    {
      private readonly Func<TModel, string> nameAccessor;
      private readonly Func<TModel, TId> idAccessor;
      private readonly IEventAggregator eventAggregator;
      private readonly Action<TModel> navigateAction;

      public class ItemViewModel : BindableBase
      {
        private IEventAggregator eventAggregator;
        private Func<TModel, TId> idAccessor;
        private TModel model;

        private string name;
        private Func<TModel, string> nameAccessor;

        public ItemViewModel(TModel model, Func<TModel, string> nameAccessor, Func<TModel, TId> idAccessor, IEventAggregator eventAggregator, Action<TModel> navigateAction)
        {
          this.model = model;
          this.nameAccessor = nameAccessor;
          this.idAccessor = idAccessor;
          this.eventAggregator = eventAggregator;

          Name = nameAccessor(model);

          var id = idAccessor(model);

          eventAggregator.GetEvent<UpdateModelEvent<TModel>>().Subscribe(_ => Name = nameAccessor(model));

          NavigateCommand = new DelegateCommand(() => navigateAction(model));
        }

        public string Name
        {
          get { return name; }
          set { SetProperty(ref name, value); }
        }

        public ICommand NavigateCommand { get; set; }
      }

      public string Name { get; set; }

      public ObservableCollection<ItemViewModel> Items { get; }  = new ObservableCollection<ItemViewModel>();

      public void Add(TModel model)
      {
        Items.Add(new ItemViewModel(model, nameAccessor, idAccessor, eventAggregator, navigateAction));
      }

      public ItemGroup(string name, IEnumerable<TModel> items, Func<TModel,string> nameAccessor, Func<TModel, TId> idAccessor, IEventAggregator eventAggregator, Action<TModel> navigateAction)
      {
        this.nameAccessor = nameAccessor;
        this.idAccessor = idAccessor;
        this.eventAggregator = eventAggregator;
        this.navigateAction = navigateAction;

        Name = name;

        foreach (var item in items)
        {
          Add(item);
        }

        eventAggregator.GetEvent<AddModelEvent<TModel>>().Subscribe(Add, ThreadOption.UIThread);
      }
    }

    public ObservableCollection<object> Groups { get; }
  }
}