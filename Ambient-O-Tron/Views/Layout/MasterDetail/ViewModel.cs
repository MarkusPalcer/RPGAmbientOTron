using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Core.Navigation;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Layout.MasterDetail
{
    [Export]
    public class ViewModel : BindableBase, IConfirmNavigationRequest
    {
        private readonly INavigationService navigationService;

        [ImportingConstructor]
        public ViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
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

        public const string MasterRegion = "MasterDetail_Master";
        public const string DetailRegion = "MasterDetail_Detail";
    }
}