﻿using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using AmbientOTron.Views.Ambience.Navigation;
using AmbientOTron.Views.Navigation;
using Core.Extensions;
using Prism.Events;

namespace AmbientOTron.Views.Ambience
{
  [Export(typeof(NavigationGroup<>))]
  public class AmbienceNavigationGroup : NavigationGroup<AmbienceNavigationViewModel>
  {
    private readonly ExportFactory<AmbienceNavigationViewModel> itemFactory;

    [ImportingConstructor]
    public AmbienceNavigationGroup(IEventAggregator eventAggregator, ExportFactory<AmbienceNavigationViewModel> itemFactory)
    {
      this.itemFactory = itemFactory;

      Name = "Ambiences";

      Items = new ObservableCollection<AmbienceNavigationViewModel>();
      eventAggregator.OnModelAdd<Core.Repository.Models.Ambience>(x => Items.Add(CreateItemViewModel(x)));
    }

    private AmbienceNavigationViewModel CreateItemViewModel(Core.Repository.Models.Ambience model)
    {
      var export = itemFactory.CreateExport();
      export.Value.Model = model;
      return export.Value;
    }
  }
}