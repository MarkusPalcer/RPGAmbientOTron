using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using AmbientOTron.Views.Properties.PropertyViewModels;
using AmbientOTron.Views.Shell;
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
    private readonly IRegionManager regionManager;

    private readonly SerialDisposable modelUpdateSubscription = new SerialDisposable();
    private readonly SerialDisposable modelRemoveSubscription = new SerialDisposable();

    private readonly IDictionary<Type, KnownPropertyType> knownPropertyTypes;

    private readonly ILog logger = LogManager.GetLogger(typeof(Repository));

    [ImportingConstructor]
    protected PropertiesViewModel(IEventAggregator eventAggregator, [ImportMany] IEnumerable<KnownPropertyType> knownPropertyTypes, IRegionManager regionManager)
    {
      this.eventAggregator = eventAggregator;
      this.regionManager = regionManager;
      this.knownPropertyTypes = knownPropertyTypes.ToDictionary(x => x.Type);
    }

    private string typeName;

    public string TypeName
    {
      get { return typeName; }
      private set { SetProperty(ref typeName, value); }
    }

    private IEnumerable<PropertyViewModel> properties = Enumerable.Empty<PropertyViewModel>();

    private object Model { get; set; }

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

      logger.InfoFormat($"Creating property pane for {modelType.FullName}");

      TypeName = modelType.GetCustomAttributes(typeof(TypeNameAttribute), false).OfType<TypeNameAttribute>().FirstOrDefault()?.Name ??
                     modelType.Name;

      var props = new List<PropertyViewModel>();

      foreach (var property in modelType.GetProperties())
      {
        {
          var attribute = property.GetCustomAttribute<SliderPropertyAttribute>();
          if (attribute != null && property.PropertyType == typeof(float))
          {
            props.Add(SliderPropertyViewModel.Create(property, Model, attribute));
            continue;
          }
        }

        {
          var attribute = property.GetCustomAttribute<PropertyAttribute>();
          if (attribute != null)
          {
            props.AddRange(AddGenericProperty(attribute, property, modelType));
            continue;
          }
        }
      }

      Properties = props;

      typeof(PropertiesViewModel).GetMethod("HookupUpdateEvent")
                               .MakeGenericMethod(modelType)
                               .Invoke(this, new object[] {});

      Properties.ForEach(x => x.SendModelUpdate = SendModelUpdate);
      Properties.ForEach(x => x.Update());
    }

    private IEnumerable<PropertyViewModel> AddGenericProperty(PropertyAttribute propertyAttribute, PropertyInfo property, Type modelType)
    {
      KnownPropertyType knownType;
      if (!knownPropertyTypes.TryGetValue(property.PropertyType, out knownType))
      {
        logger.Warn($"No view present for properties of type {property.PropertyType.Name} (property {modelType.FullName}.{property.Name})");
        yield break;
      }

      PropertyViewModel viewModel;

      try
      {
        viewModel = knownType.ViewModelFactory(property, Model);
        viewModel.Name = propertyAttribute.Name ?? property.Name;
      }
      catch (Exception ex)
      {
        logger.Error($"Could not create view model of type {property.PropertyType.Name} (property {modelType.FullName}.{property.Name}", ex);
        yield break;
      }

      yield return viewModel;
    }

    protected Action SendModelUpdate;

    // ReSharper disable once UnusedMember.Global Used through reflection
    public void HookupUpdateEvent<TModel>()
    {
      modelUpdateSubscription.Disposable = eventAggregator.OnModelUpdate(
        (TModel)Model,
        () => Properties.ForEach(x => x.Update()));

      modelRemoveSubscription.Disposable = eventAggregator.OnModelRemove(
        (TModel) Model,
        _ => regionManager.Regions[ShellViewModel.PropertiesPane].RemoveAll());

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
      Dispose(true);
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
        modelRemoveSubscription.Dispose();
      }

      // Release unmanaged resources here
    }
  }
}