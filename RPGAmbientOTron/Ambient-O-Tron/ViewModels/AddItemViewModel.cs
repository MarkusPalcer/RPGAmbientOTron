using System.Windows.Input;
using Prism.Mvvm;

namespace AmbientOTron.ViewModels
{
    public class AddItemViewModel : BindableBase
    {
        public ICommand AddCommand { get; set; }
    }
}