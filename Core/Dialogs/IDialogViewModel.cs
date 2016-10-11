using System.Threading.Tasks;

namespace Core.Dialogs
{
    public interface IDialogViewModel<TResult>
    {
        Task<TResult> Result { get; }
    }
}