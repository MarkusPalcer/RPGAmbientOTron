using Core.Repository.Sounds;
using Prism.Events;

namespace Core.Events
{
  public class TriggerSoundEvent : PubSubEvent<Sound> { }
}