using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Sounds;
using Core.Events;
using Core.Repository.Sounds;
using Prism.Events;

namespace AmbientOTron.Views.Cache
{
  [Export]
  public class CacheNavigationViewModel : NavigationItemViewModel<Core.Repository.Models.Cache, SoundNavigationViewModel>
  {
    [ImportingConstructor]
    public CacheNavigationViewModel(IEventAggregator eventAggregator)
    {
      Items = new ObservableCollection<SoundNavigationViewModel>();
      eventAggregator.GetEvent<UpdateModelEvent<Core.Repository.Models.Cache>>()
                     .Subscribe(_ => UpdateFromModel(), ThreadOption.UIThread, true, c => c.Folder == Model.Folder);
    }

    public override void SetModel(Core.Repository.Models.Cache newModel)
    {
      Model = newModel;
      UpdateFromModel();
    }

    private void UpdateFromModel()
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
    }

    private SoundNavigationViewModel CreateItemViewModel(Sound sound)
    {
      var result = new SoundNavigationViewModel();
      result.SetModel(sound);
      return result;
    }
  }
}