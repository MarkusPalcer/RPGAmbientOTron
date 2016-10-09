using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Dialogs;
using Prism.Regions;

namespace AmbientOTron.Views.Dialogs.MessageBox
{
    public static class Extensions
    {

        private static readonly Dictionary<MessageBoxButtons, DialogResult[]> ButtonResults = new Dictionary<MessageBoxButtons, DialogResult[]>
        {
            { MessageBoxButtons.OK, new[]{ DialogResult.OK } },
            { MessageBoxButtons.AbortRetryIgnore, new[] {DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore } },
            { MessageBoxButtons.OKCancel, new[] {DialogResult.OK, DialogResult.Cancel, }},
            { MessageBoxButtons.RetryCancel, new[] {DialogResult.Retry, DialogResult.Cancel, } },
            {MessageBoxButtons.YesNo, new[] {DialogResult.Yes, DialogResult.No, } },
            {MessageBoxButtons.YesNoCancel, new[] {DialogResult.Yes, DialogResult.No, DialogResult.Cancel,  }  },
        };

        public static async Task<DialogResult> ShowMessageBox(
            this IDialogService dialogService,
            string message,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            DialogResult defaultResult = DialogResult.None)
        {
            DialogResult[] buttonResults;

            if (!ButtonResults.TryGetValue(buttons, out buttonResults))
            {
                throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
            }

            return await
                ShowMessageBox(
                    dialogService,
                    message,
                    buttonResults.ToDictionary(x => x.ToString()),
                    defaultResult);
        }


        public static async Task<TResult> ShowMessageBox<TResult>(this IDialogService dialogService, string message, IEnumerable<KeyValuePair<string, TResult>> options, TResult cancelResult)
        {
            var optionsArray = options.ToArray();
            var optionsString = string.Join("|", optionsArray.Select(x => x.Key));
            var optionValues = optionsArray.Select(x => x.Value).ToArray();

            var result = await dialogService.ShowDialog<View, int?>(new NavigationParameters
            {
                {"message", message},
                {"options", optionsString}
            });

            return result.HasValue ? optionValues[result.Value] : cancelResult;
        }
    }
}
