using System;
using System.Collections.Generic;
using Prism.Commands;

namespace Core.Navigation
{
    public interface INavigationService
    {
        DelegateCommand CreateNavigationCommand(IEnumerable<NavigationRequest> requests);
        DelegateCommand CreateNavigationCommand(NavigationRequest request);
        DelegateCommand CreateNavigationCommand(Uri view, string region);
        DelegateCommand CreateNavigationCommand<TView>(string region);
        void Navigate(NavigationRequest request);
    }
}