using System.IO;

namespace Core.Repository.Sources
{
  public class AudioFile : Source
  {
    public AudioFile(string fullPath)
      : base(() => File.OpenRead(fullPath))
    {
    }
  }
}