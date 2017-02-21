using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Extensions
{
  public static class SemaphoreSlimExtensions
  {
    public static async Task<IDisposable> ProtectAsync(this SemaphoreSlim x)
    {
      await x.WaitAsync();
      return Disposable.Create(() => x.Release());
    }

    public static IDisposable Protect(this SemaphoreSlim x)
    {
      x.Wait();
      return Disposable.Create(() => x.Release());
    }
  }
}