using System;
using System.IO;
using System.Security.Cryptography;

namespace Core.Repository.Sources
{
  public abstract class Source : ISource
  {
    private readonly Func<Stream> open;

    protected Source(Func<Stream> open)
    {
      this.open = open;

      var hasher = SHA256.Create();
      var hash = hasher.ComputeHash(Open());
      Hash = Convert.ToBase64String(hash);
    }

    public string Hash { get; }

    public Stream Open()
    {
      return open();
    }
  }
}