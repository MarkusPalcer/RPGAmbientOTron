using Core.Repository.Sounds;
using Prism.Events;

namespace Core.Events
{
  public class ManualTriggerEvent<TModel> : PubSubEvent<TModel> { }
}