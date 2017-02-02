using System.Collections.Generic;

namespace AmbientOTron.Views.Navigation
{
  public class NavigationGroup<TItem>
  {
    public string Name { get; set; }
    public IEnumerable<TItem> Items { get; set; } 
  }
}