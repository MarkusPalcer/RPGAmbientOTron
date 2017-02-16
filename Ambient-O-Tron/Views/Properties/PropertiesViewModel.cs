using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using AmbientOTron.Views.Properties.PropertyViewModels;
using Core.Events;
using Core.Extensions;
using Core.Repository;
using Core.Repository.Attributes;
using log4net;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace AmbientOTron.Views.Properties
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class PropertiesViewModel : BindableBase, INavigationAware, IDisposable
  {
    public struct KnownPropertyType
    {
      public Type Type;
      public Func<PropertyInfo, object, PropertyViewModel> ViewModelFactory;
    }

    private readonly IEventAggregator eventAggregator;

    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();

    private readonly IDictionary<Type, KnownPropertyType> knownPropertyTypes;

    private readonly ILog logger = LogManager.GetLogger(typeof(Repository));

    [ImportingConstructor]
    protected PropertiesViewModel(IEventAggregator eventAggregator, [ImportMany] IEnumerable<KnownPropertyType> knownPropertyTypes)
    {
      this.eventAggregator = eventAggregator;
      this.knownPropertyTypes = knownPropertyTypes.ToDictionary(x => x.Type);
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

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      Model = navigationContext.GetModel<object>();

      var modelType = Model.GetType();

      logger.InfoFormat("Creating property pane for {1}", modelType.FullName);

      TypeName = modelType.GetCustomAttributes(typeof(TypeNameAttribute), false).OfType<TypeNameAttribute>().FirstOrDefault()?.Name ??
                     modelType.Name;

      var props = new List<PropertyViewModel>();

      foreach (var property in modelType.GetProperties())
      {
        var propertyAttribute = property.GetCustomAttributes<PropertyAttribute>().FirstOrDefault();

        if (propertyAttribute == null)
        {
          continue;
        }

        KnownPropertyType knownType;
        if (!knownPropertyTypes.TryGetValue(property.PropertyType, out knownType))
        {
          logger.Warn($"No view present for properties of type {property.PropertyType.Name} (property {modelType.FullName}.{property.Name})");
          continue;
        }

        try
        {
          var viewModel = knownType.ViewModelFactory(property, Model);
          viewModel.Name = propertyAttribute.Name ?? property.Name;
          props.Add(viewModel);
        }
        catch (Exception ex)
        {
          logger.Error($"Could not create view model of type {property.PropertyType.Name} (property {modelType.FullName}.{property.Name}", ex);
        }
      }

      Properties = props;

      typeof(PropertiesViewModel).GetMethod("HookupUpdateEvent")
                               .MakeGenericMethod(modelType)
                               .Invoke(this, new object[] {});

      Properties.ForEach(x => x.SendModelUpdate = SendModelUpdate);
      Properties.ForEach(x => x.Update());
    }

    protected Action SendModelUpdate;

    // ReSharper disable once UnusedMember.Global Used through reflection
    public void HookupUpdateEvent<TModel>()
    {
      modelUpdateSubscription.Disposable = eventAggregator.OnModelUpdate(
        Model,
        () => Properties.ForEach(x => x.Update()));

      SendModelUpdate = () => eventAggregator.ModelUpdated((TModel) Model);
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