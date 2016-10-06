using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> src, Action<T> action)
        {
            var exceptions = new List<Exception>();

            foreach (var item in src)
            {
                try
                {
                    action(item);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> src)
        {
            return new ObservableCollection<T>(src);
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> src, Func<T, Task> action)
        {
            foreach (var item in src)
            {
                await action(item);
            }
        }
    }
}
