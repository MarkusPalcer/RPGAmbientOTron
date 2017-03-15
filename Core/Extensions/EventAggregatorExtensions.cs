using System;
using Core.Events;
using Core.Repository.Sounds;
using Prism.Events;

namespace Core.Extensions
{
  public static class EventAggregatorExtensions
  {
    public static void ModelUpdated<TModel>(this IEventAggregator eventAggregator, TModel model)
    {
      eventAggregator.GetEvent<UpdateModelEvent<TModel>>().Publish(model);
    }

    public static void ModelAdded<TModel>(this IEventAggregator eventAggregator, TModel model)
    {
      eventAggregator.GetEvent<AddModelEvent<TModel>>().Publish(model);
    }

    public static void ModelRemoved<TModel>(this IEventAggregator eventAggregator, TModel model)
    {
      eventAggregator.GetEvent<RemoveModelEvent<TModel>>().Publish(model);
    }

    public static SubscriptionToken OnModelUpdate<TModel>(
      this IEventAggregator eventAggregator,
      TModel model,
      Action handler)
    {
      return eventAggregator.OnModelUpdate(model, _ => handler());
    }

    public static SubscriptionToken OnModelUpdate<TModel>(this IEventAggregator eventAggregator, TModel model, Action<TModel> handler)
    {
      return eventAggregator.GetEvent<UpdateModelEvent<TModel>>()
                     .Subscribe(handler, ThreadOption.UIThread, true, m => ReferenceEquals(m, model));
    }

    public static SubscriptionToken OnModelRemove<TModel>(this IEventAggregator eventAggregator, TModel model, Action<TModel> handler, ThreadOption threadOption = ThreadOption.UIThread)
    {
      return eventAggregator.GetEvent<RemoveModelEvent<TModel>>()
                     .Subscribe(handler, threadOption, true, m => ReferenceEquals(m, model));
    }

    public static SubscriptionToken OnModelRemove<TModel>(this IEventAggregator eventAggregator, Action<TModel> handler, ThreadOption threadOption = ThreadOption.UIThread)
    {
      return eventAggregator.GetEvent<RemoveModelEvent<TModel>>()
                     .Subscribe(handler, threadOption, true);
    }

    public static SubscriptionToken OnModelAdd<TModel>(this IEventAggregator eventAggregator, Action<TModel> handler)
    {
      return eventAggregator.GetEvent<AddModelEvent<TModel>>()
                     .Subscribe(handler, ThreadOption.UIThread, true);
    }

    public static void Trigger(this IEventAggregator eventAggregator, SoundModel sound)
    {
      eventAggregator.GetEvent<ManualTriggerEvent<SoundModel>>().Publish(sound);
    }
  }
}