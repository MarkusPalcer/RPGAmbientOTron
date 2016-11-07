using System.Runtime.CompilerServices;

namespace Core.Audio
{
  public interface IPlayback
  {
    TaskAwaiter GetAwaiter();
  }
}