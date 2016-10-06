using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Regions;

namespace Core.Navigation
{
    public interface INavigationService
    {
        DelegateCommand CreateNavigationCommand(IEnumerable<NavigationRequest> requests);
        DelegateCommand CreateNavigationCommand(NavigationRequest request);
        DelegateCommand CreateNavigationCommand(string region, Uri view, NavigationParameters parameters = null);
        DelegateCommand CreateNavigationCommand<TView>(string region, NavigationParameters parameters = null);

        Task<bool> NavigateAsync(NavigationRequest request);

        Task<bool> NavigateAsync(string region, Uri view, NavigationParameters parameters = null);

        Task<bool> NavigateAsync<TView>(string region, NavigationParameters parameters = null);
    }
}