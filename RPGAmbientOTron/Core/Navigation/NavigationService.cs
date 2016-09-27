using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Core.Extensions;
using Prism.Commands;
using Prism.Logging;
using Prism.Regions;

namespace Core.Navigation
{
    [Export(typeof(INavigationService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class NavigationService : INavigationService {
        private readonly IRegionManager regionManager;
        private readonly ILoggerFacade logger;

        [ImportingConstructor]
        public NavigationService(IRegionManager regionManager, ILoggerFacade logger)
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
                    logger.LogException(result.Error, $"Error while executing navigation request {result.Context.NavigationService.Region.Name} -> {result.Context.Uri}");
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

    public class NavigationRequestCollection : Collection<NavigationRequest>
    {
        public void Add(string region, Uri view, NavigationParameters parameters = null)
        {
            Add(new NavigationRequest
            {
                Region = region,
                View = view, 
                Parameters =  parameters
            });
        }

        public void Add(string region, Type viewType, NavigationParameters parameters = null)
        {
            Add(new NavigationRequest
            {
                Region = region,
                View = new Uri(viewType.FullName, UriKind.Relative),
                Parameters = parameters
            });
        }
    }

   
}