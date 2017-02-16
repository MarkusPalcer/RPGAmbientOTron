using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Sounds;
using Core.Extensions;
using Core.Repository.Sounds;
using Prism.Events;

namespace AmbientOTron.Views.Cache
{
  [Export]
  public class CacheNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.Cache, SoundNavigationViewModel>
  {
    private readonly IEventAggregator eventAggregator;
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    [ImportingConstructor]
    public CacheNavigationViewModel(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;
      Items = new ObservableCollection<SoundNavigationViewModel>();
    }

    protected override void OnModelSet(Core.Repository.Models.Cache newModel)
    {
      eventAggregator.OnModelUpdate(Model, UpdateFromModel);
    }

    protected override async void UpdateFromModel()
    {
      using (await semaphore.ProtectAsync())
      {
        Name = Model.Name;

        var toRemove = Items.ToDictionary(x => x.Model.Hash);

        foreach (var sound in Model.Sounds)
        {
          if (toRemove.ContainsKey(sound.Hash))
          {
            toRemove.Remove(sound.Hash);
          }
          else
          {
            Items.Add(CreateItemViewModel(sound));
          }
        }

        foreach (var removeValue in toRemove.Values)
        {
          Items.Remove(removeValue);
        }
      }
    }

    private SoundNavigationViewModel CreateItemViewModel(Sound sound)
    {
      return new SoundNavigationViewModel
      {
        Model = sound
      };
    }
  }
}