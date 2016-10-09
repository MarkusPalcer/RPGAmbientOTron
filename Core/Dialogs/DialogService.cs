using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Core.Navigation;
using Prism.Regions;

namespace Core.Dialogs
{
    [Export(typeof(IDialogService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DialogService : IDialogService
    {
        private readonly IRegionManager regionManager;
        private readonly INavigationService navigationService;

        public const string DialogRegionName = "5FC69184-480F-4AD3-B86B-FAD435EB04B3";

        [ImportingConstructor]
        public DialogService(IRegionManager regionManager, INavigationService navigationService)
        {
            this.regionManager = regionManager;
            this.navigationService = navigationService;
        }

        public async Task<TResult> ShowDialog<TView, TResult>(NavigationParameters parameters = null) where TView : class, IDialogView<TResult>
        {
            var region = regionManager.Regions[DialogRegionName];
            var oldDialog = region.Views.FirstOrDefault();

            region.RemoveAll();

            try
            {
                var result = await navigationService.NavigateAsync<TView>(DialogRegionName, parameters);
                if (result == false)
                {
                    throw new TaskCanceledException();
                }

                var newDialog = region.Views.FirstOrDefault() as TView;
                var viewModel = newDialog?.ViewModel;

                if (viewModel != null)
                {
                    return await viewModel.Result;
                }
                else
                {
                    throw new NullReferenceException("The view model of the dialog view must not be null.");
                }
            }
            finally
            {
                region.RemoveAll();
                if (oldDialog != null)
                {
                    region.Add(oldDialog);
                    region.Activate(oldDialog);
                }
            }
        }
    }
}