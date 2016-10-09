using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Dialogs;
using Prism.Commands;
using Prism.Regions;

namespace AmbientOTron.Views.Dialogs.MessageBox
{
    [Export]
    public class ViewModel : IDialogViewModel<int?>, INavigationAware
    {
        private TaskCompletionSource<int?> taskCompletionSource;

        public ViewModel()
        {

        }

        public IEnumerable<OptionViewModel> Options { get; private set; }

        public string Message { get; private set; }

        #region Implementation of IDialogViewModel<int?>

        public Task<int?> Result => taskCompletionSource.Task;

        #endregion

        public class OptionViewModel
        {
            public string Text { get; set; }

            public ICommand SelectCommand { get; set; }
        }

        #region Implementation of INavigationAware

        [SuppressMessage("ReSharper", "NotResolvedInText", Justification = "The arguments are read from the navigationContext")]
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            taskCompletionSource = new TaskCompletionSource<int?>();

            Message = navigationContext.Parameters["message"] as string;
            if (Message == null)
            {
                taskCompletionSource.SetException(new ArgumentNullException("message", @"The dialog message must be set."));
                return;
            }

            var optionsString = navigationContext.Parameters["options"] as string;
            if (optionsString == null)
            {
                taskCompletionSource.SetException(new ArgumentNullException("options", @"The selectable options must be set."));
                return;
            }

            Options = optionsString.Split('|').Select((x, i) =>
                                                          new OptionViewModel
                                                          {
                                                              Text = x,
                                                              SelectCommand = new DelegateCommand(() => taskCompletionSource.SetResult(i))
                                                          });


        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            taskCompletionSource.TrySetResult(null);
        }

        #endregion
    }
}