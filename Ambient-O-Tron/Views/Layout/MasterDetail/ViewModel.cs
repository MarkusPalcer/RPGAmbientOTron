using System;
using System.ComponentModel.Composition;
using Core.Navigation;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Layout.MasterDetail
{
    [Export]
    public class ViewModel : BindableBase, IConfirmNavigationRequest
    {
        private readonly INavigationService navigationService;
        private string myRegion = null;

        [ImportingConstructor]
        public ViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Uri.Equals(new Uri(typeof(View).FullName, UriKind.Relative)))
            {
                myRegion = navigationContext.NavigationService.Region.Name;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (navigationContext.Uri.Equals(new Uri(typeof(View).FullName, UriKind.Relative)))
            {
                myRegion = null;
            }
        }

        public async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            // I'm only interested when my region gets new content
            if (myRegion == navigationContext.NavigationService.Region.Name)
            {
                try
                {
                    continuationCallback(
                                         await navigationService.NavigateAsync<Empty>(MasterRegion) &&
                                         await navigationService.NavigateAsync<Empty>(DetailRegion));
                }
                catch (Exception)
                {
                    continuationCallback(false);
                }
            }
            else
            {
                continuationCallback(true);
            }
        }

        public const string MasterRegion = "MasterDetail_Master";
        public const string DetailRegion = "MasterDetail_Detail";
    }
}