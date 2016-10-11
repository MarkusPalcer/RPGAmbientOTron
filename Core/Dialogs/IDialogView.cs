namespace Core.Dialogs
{
    public interface IDialogView<TResult>
    {
        IDialogViewModel<TResult> ViewModel { get; }
    }
}