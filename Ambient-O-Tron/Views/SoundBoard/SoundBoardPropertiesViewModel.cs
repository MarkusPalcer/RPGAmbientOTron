using System;
using System.ComponentModel.Composition;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.SoundBoard
{
  [Export]
  public class SoundBoardPropertiesViewModel : BindableBase, IConfirmNavigationRequest, IDisposable
  {
    [ImportingConstructor]
    public SoundBoardPropertiesViewModel(SoundBoardViewModel itemViewModel)
    {
      ViewModel = itemViewModel;
    }
    
    public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
      ViewModel.ConfirmNavigationRequest(navigationContext,continuationCallback);
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return ViewModel.IsNavigationTarget(navigationContext);
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
      ViewModel.OnNavigatedFrom(navigationContext);
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      var targetViewModel = navigationContext.Parameters["ViewModel"] as SoundBoardViewModel;
      if (targetViewModel != null)
      {
        ViewModel.Dispose();
        ViewModel = targetViewModel;
      }
      else
      {
        ViewModel.OnNavigatedTo(navigationContext);
      }
    }

    private SoundBoardViewModel viewModel;
    public SoundBoardViewModel ViewModel
    {
      get { return viewModel; }
      set { SetProperty(ref viewModel, value); }
    }

    #region IDisposable

    public void Dispose()
    {
      viewModel?.Dispose();
    }

    #endregion
  }
}