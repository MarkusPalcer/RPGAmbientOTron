using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
            return new DelegateCommand(() => ExecuteNavigationCommand(requests));
        }

        public DelegateCommand CreateNavigationCommand(NavigationRequest request)
        {
            return CreateNavigationCommand(new[] { request });
        }

        public DelegateCommand CreateNavigationCommand(Uri view, string region)
        {
            return CreateNavigationCommand(new[] { new NavigationRequest(view, region), });
        }

        public DelegateCommand CreateNavigationCommand<TView>(string region)
        {
            return CreateNavigationCommand(new[] { NavigationRequest.Create<TView>(region) });
        }

        private void ExecuteNavigationCommand(IEnumerable<NavigationRequest> requests)
        {
            requests.ForEach(Navigate);
        }

        public void Navigate(NavigationRequest request)
        {
            regionManager.RequestNavigate(request.Region, request.View, request.Callback ?? DefaultCallback, request.Parameters);
        }

        private void DefaultCallback(NavigationResult result)
        {
            if (result.Error != null)
            {
                logger.LogException(result.Error, $"Error while executing navigation request {result.Context.NavigationService.Region.Name} -> {result.Context.Uri}");
            }
        }
    }
}