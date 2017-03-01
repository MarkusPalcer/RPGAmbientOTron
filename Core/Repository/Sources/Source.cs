using System;
using System.IO;
using System.Security.Cryptography;
using Core.Audio;
using NAudio.Wave;

namespace Core.Repository.Sources
{
  public abstract class Source : ISource
  {
    private readonly WeakReference<byte[]> cache = new WeakReference<byte[]>(null);
    private WaveFormat format;

    private readonly Func<Stream> open;

    protected Source(Func<Stream> open)
    {
      this.open = open;

      var hasher = SHA256.Create();
      var hash = hasher.ComputeHash(Open());
      Hash = Convert.ToBase64String(hash);
    }

    public string Hash { get; }

    public WaveStream Open()
    {
      lock (cache)
      {
        byte[] data;
        if (cache.TryGetTarget(out data))
        {
          return CreateWaveStream(data);
        }

        using (var src = new Mp3FileReader(open()))
        {
          data = new byte[src.Length];
          cache.SetTarget(data);
          src.Read(data, 0, data.Length);
          format = src.WaveFormat;

          return CreateWaveStream(data);
        }
      }
    }

    private WaveStream CreateWaveStream(byte[] data)
    {
      return new RawSourceWaveStream(new MemoryStream(data), format);
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
      return Equals((Source)obj);
    }

    public override int GetHashCode()
    {
      return Hash?.GetHashCode() ?? 0;
    }
  }
}