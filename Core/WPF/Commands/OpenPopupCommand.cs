using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Prism.Commands;

namespace Core.WPF.Commands
{
  public class OpenPopupCommand : DependencyObject, ICommand
  {
    public OpenPopupCommand()
    {
      internalCommand = new DelegateCommand(() => Popup.IsOpen = true, () => Popup != null);
    }

    private readonly DelegateCommand internalCommand;

    public static readonly DependencyProperty PopupProperty = DependencyProperty.Register("Popup", typeof(Popup), typeof(OpenPopupCommand), new PropertyMetadata(default(Popup), OnPopupChanged));

    private static void OnPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as OpenPopupCommand)?.internalCommand.RaiseCanExecuteChanged();
    }

    public Popup Popup
    {
      get { return (Popup) GetValue(PopupProperty); }
      set { SetValue(PopupProperty, value); }
    }

    public bool CanExecute(object parameter)
    {
      return ((ICommand) internalCommand).CanExecute(parameter);
    }

    public void Execute(object parameter)
    {
      ((ICommand) internalCommand).Execute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
      add { ((ICommand) internalCommand).CanExecuteChanged += value; }
      remove { ((ICommand) internalCommand).CanExecuteChanged -= value; }
    }
  }
}