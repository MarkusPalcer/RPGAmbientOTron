using Core.Repository.Models;
using Prism.Events;

namespace Core.Events
{
  public class InitializeSoundBoardEvent : PubSubEvent<SoundBoardModel>
  {
  }
}