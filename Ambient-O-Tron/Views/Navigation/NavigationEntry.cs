using System;
using Core.Extensions;

namespace AmbientOTron.Views.Navigation
{
  public class NavigationEntry<TViewModel> : IDisposable
  {
    private readonly TViewModel viewModel;
    private readonly IDisposable[] disposables;

    public NavigationEntry(TViewModel viewModel, IDisposable[] disposables)
    {
      this.viewModel = viewModel;
      this.disposables = disposables;
    }

    public TViewModel ViewModel => viewModel;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        disposables.ForEach(x => x.Dispose());
        (viewModel as IDisposable)?.Dispose();
      }

      // Release unmanaged resources here
    }
  }
}