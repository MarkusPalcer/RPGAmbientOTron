using NAudio.Wave;

namespace Core.Repository.Sources
{
  internal interface ISource
  {
    string Hash { get; }

    WaveStream Open();
  }
}