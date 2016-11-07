﻿using System.Reactive;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Core.Audio
{
  /// <summary>
  /// Represents a file currently being played back
  /// </summary>
  internal class Playback : IPlayback
  {
    private WaveOut waveOut;
    private AudioFileReader audioFileReader;
    private TaskCompletionSource<Unit> taskCompletionSource;

    public Playback(string fileName)
    {
      taskCompletionSource = new TaskCompletionSource<Unit>();

      waveOut = new WaveOut();
      audioFileReader = new AudioFileReader(fileName);
      waveOut.Init(audioFileReader);
      waveOut.PlaybackStopped += HandlePlaybackStopped;
      waveOut.Play();
    }

    private void HandlePlaybackStopped(object sender, StoppedEventArgs e)
    {
      waveOut.Dispose();
      audioFileReader.Dispose();

      if (e.Exception == null)
      {
        taskCompletionSource.SetResult(Unit.Default);
      }
      else
      {
        taskCompletionSource.SetException(e.Exception);
      }
    }

    #region Implementation of IAsyncOperation

    public TaskAwaiter GetAwaiter()
    {
      return ((Task)taskCompletionSource.Task).GetAwaiter();
    }

    #endregion
  }
}