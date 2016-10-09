using System.Threading.Tasks;

namespace Core.Dialogs
{
    public interface IDialogView<TResult>
    {
        IDialogViewModel<TResult> ViewModel { get; }
    }

    public interface IDialogViewModel<TResult>
    {
        Task<TResult> Result { get; }
    }
}