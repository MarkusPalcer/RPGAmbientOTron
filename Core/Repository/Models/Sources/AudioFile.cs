using System.IO;
using Newtonsoft.Json;

namespace Core.Repository.Models.Sources
{
  public class AudioFile 
  {
    public string FullPath { get; set; }

    public string Name { get; set; }

    #region Implementation of ISource

    public Stream Open()
    {
      return File.OpenRead(FullPath);
    }

    #endregion
  }
}