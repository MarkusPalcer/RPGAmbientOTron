using System;
using Prism.Regions;

namespace Core.Navigation
{
    public class NavigationRequest
    {
        public NavigationParameters Parameters { get; set; } = null;

        public Uri View { get; set; }
        public string Region { get; set; }
    }
}
