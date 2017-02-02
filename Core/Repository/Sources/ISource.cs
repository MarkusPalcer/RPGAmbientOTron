using System.IO;

namespace Core.Repository.Sources
{
  internal interface ISource
  {
    string Hash { get; }

    Stream Open();
  }
}