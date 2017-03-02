using Prism.Events;

namespace Core.Events
{
    public class AddModelEvent<TModel> : PubSubEvent<TModel> { }
    public class RemoveModelEvent<TModel> : PubSubEvent<TModel> { }
}