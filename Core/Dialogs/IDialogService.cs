using System.Threading.Tasks;
using Prism.Regions;

namespace Core.Dialogs
{
    public interface IDialogService {
        Task<TResult> ShowDialog<TView, TResult>(NavigationParameters parameters = null) where TView: class, IDialogView<TResult>;
    }
}