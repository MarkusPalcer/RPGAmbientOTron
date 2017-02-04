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

    protected bool Equals(Source other)
    {
      return string.Equals(Hash, other.Hash);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((Source) obj);
    }

    public override int GetHashCode()
    {
      return Hash?.GetHashCode() ?? 0;
    }
  }
}