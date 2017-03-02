using System.Windows.Input;
using Prism.Commands;

namespace AmbientOTron.Views.Navigation
{
  public class NavigationItemContextMenuEntry
  {

    public NavigationItemContextMenuEntry(string caption, DelegateCommand command)
    {
      Caption = caption;
      Command = command;
    }

    public string Caption { get; set; }
    public ICommand Command { get; set; }
  }
}