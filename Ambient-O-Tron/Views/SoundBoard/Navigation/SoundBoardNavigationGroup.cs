using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using AmbientOTron.Views.Navigation;
using AmbientOTron.Views.Shell;
using Core.Extensions;
using Core.Navigation;
using Core.Repository.Models;
using Prism.Events;

namespace AmbientOTron.Views.SoundBoard.Navigation
{
  [Export(typeof(NavigationGroup<>))]
  public class SoundBoardNavigationGroup : NavigationGroup<SoundBoardNavigationViewModel>
  {
    private class NavigationEntry : IDisposable
    {
      private readonly IDisposable exportDisposable;
      private readonly IDisposable removalSubscription;

      public NavigationEntry(IDisposable exportDisposable, IDisposable removalSubscription, SoundBoardNavigationViewModel viewModel)
      {
        this.exportDisposable = exportDisposable;
        this.removalSubscription = removalSubscription;
        ViewModel = viewModel;
      }

      public SoundBoardNavigationViewModel ViewModel { get; }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Releases unmanaged and - optionally - managed resources.
      /// </summary>
      /// <param name="disposing">
      ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
      /// </param>
      protected virtual void Dispose(bool disposing)
      {
        if (disposing)
        {
          exportDisposable?.Dispose();
          removalSubscription?.Dispose();
        }

        // Release unmanaged resources here
      }
    }

    private readonly IEventAggregator eventAggregator;
    private readonly ExportFactory<SoundBoardNavigationViewModel> itemFactory;
    private readonly Dictionary<SoundBoardModel, NavigationEntry> modelEntries = new Dictionary<SoundBoardModel, NavigationEntry>();

    [ImportingConstructor]
    public SoundBoardNavigationGroup(IEventAggregator eventAggregator, ExportFactory<SoundBoardNavigationViewModel> itemFactory, INavigationService navigationService)
    {
      this.eventAggregator = eventAggregator;
      this.itemFactory = itemFactory;

      Name = "SoundBoards";

      Items = new ObservableCollection<SoundBoardNavigationViewModel>();

      eventAggregator.OnModelAdd<SoundBoardModel>(x => Items.Add(CreateItemViewModel(x)));

      ContextMenuEntries = new[]
      {
        new NavigationItemContextMenuEntry(
          "New SoundBoard",
          navigationService.CreateNavigationCommand<NewSoundBoardView>(ShellViewModel.LowerPane)),
      };
    }

    private SoundBoardNavigationViewModel CreateItemViewModel(SoundBoardModel model)
    {
      var export = itemFactory.CreateExport();
      export.Value.Model = model;

      var removalSubscription = eventAggregator.OnModelRemove(model, RemoveItemViewModel);

      modelEntries[model] = new NavigationEntry(export,removalSubscription, export.Value);

      return export.Value;
    }

    private void RemoveItemViewModel(SoundBoardModel model)
    {
      var entry = modelEntries[model];
      modelEntries.Remove(model);

      Items.Remove(entry.ViewModel);
      entry.Dispose();
    }
  }
}