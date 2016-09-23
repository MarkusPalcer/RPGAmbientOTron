using System;
using Prism.Regions;

namespace Core.Navigation
{
    public class NavigationRequest
    {
        public Action<NavigationResult> Callback { get; set; } = null;

        public NavigationParameters Parameters { get; set; } = null;

        public NavigationRequest(Uri view, string region)
        {
            View = view;
            Region = region;
        }

        public NavigationRequest(Type view, string region) : this(new Uri(view.FullName, UriKind.Relative), region) {}

        public static NavigationRequest Create<TView>(string region)
        {
            return new NavigationRequest(typeof(TView), region);
        }

        public Uri View { get; }
        public string Region { get; }
    }
}
