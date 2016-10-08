using Prism.Events;

namespace Core.Events
{
    public class AddModelEvent<TModel> : PubSubEvent<TModel>
    {
        
    }

    public class UpdateModelEvent<TModel> : PubSubEvent<TModel> { }
}