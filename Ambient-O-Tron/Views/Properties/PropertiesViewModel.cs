using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using AmbientOTron.Views.Properties.PropertyViewModels;
using Core.Events;
using Core.Extensions;
using Core.Repository.Attributes;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Properties
{
  [Export]
  public abstract class PropertiesViewModel : BindableBase, INavigationAware, IDisposable
  {
    private readonly IEventAggregator eventAggregator;

    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();

    protected PropertiesViewModel(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;
    }

    private string typeName;

    public string TypeName
    {
      get { return typeName; }
      private set { SetProperty(ref typeName, value); }
    }

    private IEnumerable<PropertyViewModel> properties = Enumerable.Empty<PropertyViewModel>();
    protected object Model { get; private set; }

    public IEnumerable<PropertyViewModel> Properties  
    {
      get { return properties; }
      protected set
      {
        SetProperty(ref properties, value);
      }
    }

    protected abstract void InitializeProperties();

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      Model = navigationContext.Parameters["model"];

      var modelType = Model.GetType();
      TypeName = modelType.GetCustomAttributes(typeof(TypeNameAttribute), false).OfType<TypeNameAttribute>().FirstOrDefault()?.Name ??
                     modelType.Name;

      InitializeProperties();

      typeof(PropertiesViewModel).GetMethod("HookupUpdateEvent")
                               .MakeGenericMethod(modelType)
                               .Invoke(this, new object[] {});

      Properties.ForEach(x => x.SendModelUpdate = SendModelUpdate);
      Properties.ForEach(x => x.Update());
    }

    protected Action SendModelUpdate;

    public void HookupUpdateEvent<TModel>()
    {
      modelUpdateSubscription.Disposable =
        eventAggregator.GetEvent<UpdateModelEvent<TModel>>()
                       .Subscribe(
                         _ => Properties.ForEach(x => x.Update()),
                         ThreadOption.UIThread,
                         true,
                         m => ReferenceEquals(m, Model));

      SendModelUpdate = () => eventAggregator.GetEvent<UpdateModelEvent<TModel>>().Publish((TModel)Model);
    }

    public virtual bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return false;
    }

    public virtual void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }


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
        modelUpdateSubscription.Dispose();
      }

      // Release unmanaged resources here
    }
  }
}