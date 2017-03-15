using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Core.Repository.Models;
using Prism.Events;

namespace AmbientOTron.Views.Ambience.Navigation
{
  [Export(typeof(NavigationGroup<>))]
  public class AmbienceNavigationGroup : NavigationGroup<AmbienceNavigationViewModel>
  {
    private readonly IEventAggregator eventAggregator;
    private readonly ExportFactory<AmbienceNavigationViewModel> itemFactory;

    private readonly Dictionary<AmbienceModel, NavigationEntry<AmbienceNavigationViewModel>>  ambienceModelDisposables = new Dictionary<AmbienceModel, NavigationEntry<AmbienceNavigationViewModel>>();

    [ImportingConstructor]
    public AmbienceNavigationGroup(IEventAggregator eventAggregator, ExportFactory<AmbienceNavigationViewModel> itemFactory)
    {
      this.eventAggregator = eventAggregator;
      this.itemFactory = itemFactory;

      Name = "Ambiences";

      Items = new ObservableCollection<AmbienceNavigationViewModel>();
      eventAggregator.OnModelAdd<AmbienceModel>(x => Items.Add(CreateItemViewModel(x)));
    }

    private AmbienceNavigationViewModel CreateItemViewModel(AmbienceModel model)
    {
      var export = itemFactory.CreateExport();
      export.Value.Model = model;

      var removalSubscription = eventAggregator.OnModelRemove(model, RemoveItemViewModel);

      ambienceModelDisposables[model] = new NavigationEntry<AmbienceNavigationViewModel>(
        export.Value,
        new IDisposable[] {removalSubscription, export});

      return export.Value;
    }

    private void RemoveItemViewModel(AmbienceModel model)
    {
      var entry = ambienceModelDisposables[model];
      ambienceModelDisposables.Remove(model);
      Items.Remove(entry.ViewModel);
      entry.Dispose();
    }
  }
}