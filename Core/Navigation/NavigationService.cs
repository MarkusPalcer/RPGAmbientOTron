using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Logging;
using Prism.Commands;
using Prism.Regions;

namespace Core.Navigation
{
    [Export(typeof(INavigationService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class NavigationService : INavigationService {
        private readonly IRegionManager regionManager;
        private readonly ILoggingService logger;

        [ImportingConstructor]
        public NavigationService(IRegionManager regionManager, ILoggingService logger)
        {
            this.regionManager = regionManager;
            this.logger = logger;
        }

        public DelegateCommand CreateNavigationCommand(IEnumerable<NavigationRequest> requests)
        {
            return new DelegateCommand(async () => await ExecuteNavigationCommand(requests));
        }

        public DelegateCommand CreateNavigationCommand(NavigationRequest request)
        {
            return CreateNavigationCommand(new[] {request});
        }

        public DelegateCommand CreateNavigationCommand(string region, Uri view, NavigationParameters parameters = null)
        {
            return CreateNavigationCommand(new[] { new NavigationRequest
            {
                Region = region,
                View = view,
                Parameters = parameters
            } });
        }

        public DelegateCommand CreateNavigationCommand<TView>(string region, NavigationParameters parameters = null)
        {
            return CreateNavigationCommand(region,new Uri(typeof(TView).FullName, UriKind.Relative), parameters );
        }

        public Task<bool> NavigateAsync(NavigationRequest request)
        {
            return NavigateAsync(request.Region, request.View, request.Parameters);
        }

        private async Task ExecuteNavigationCommand(IEnumerable<NavigationRequest> requests)
        {
            await requests.ForEachAsync(r => NavigateAsync(r.Region, r.View, r.Parameters));
        }

        public Task<bool> NavigateAsync(string region, Uri view, NavigationParameters parameters = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            Action<NavigationResult> resultHandler = result =>
            {
                if (result.Error != null)
                {
                    logger.Error<NavigationService>($"Error while executing navigation request {result.Context.NavigationService.Region.Name} -> {result.Context.Uri}", result.Error);
                    tcs.SetException(result.Error);
                }
                else
                {
                    tcs.SetResult(result.Result.HasValue && result.Result.Value);
                }
            };

            regionManager.RequestNavigate(region, view, resultHandler,  parameters);

            return tcs.Task;
        }

        public Task<bool> NavigateAsync<TView>(string region, NavigationParameters parameters = null)
        {
            return NavigateAsync(region, new Uri(typeof(TView).FullName, UriKind.Relative), parameters);
        }

    }
}