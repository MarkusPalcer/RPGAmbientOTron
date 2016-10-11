using Prism.Events;

namespace Core.Events
{
    public class UpdateModelEvent<TModel> : PubSubEvent<TModel> { }
}