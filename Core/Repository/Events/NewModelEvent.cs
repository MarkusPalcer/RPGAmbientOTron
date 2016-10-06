using Prism.Events;

namespace Core.Repository.Events
{
  public class NewModelEvent<TModel> : EventBase
  {
    private TModel Model { get; set; }
  }

  public class ModelDeletedEvent<TModel> : EventBase
  {
    private TModel Model { get; set; }
  }

  public class ModelUpdatedEvent<TModel> : EventBase
  {
    private TModel Modell { get; set; }
  }
}