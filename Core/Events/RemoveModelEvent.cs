using Prism.Events;

namespace Core.Events
{
  public class RemoveModelEvent<TModel> : PubSubEvent<TModel> { }
}