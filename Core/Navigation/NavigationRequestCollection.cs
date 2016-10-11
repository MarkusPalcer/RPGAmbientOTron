using System;
using System.Collections.ObjectModel;
using Prism.Regions;

namespace Core.Navigation
{
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