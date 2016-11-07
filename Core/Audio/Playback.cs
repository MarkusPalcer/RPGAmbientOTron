using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using log4net.Core;
using NAudio.Wave;

namespace Core.Audio
{
  /// <summary>
  /// Represents a file currently being played back
  /// </summary>
  internal class Playback : IPlayback
  {
    private WaveOut waveOut;
    private ProgressReportingAudioFileReader audioFileReader;
    private TaskCompletionSource<Unit> taskCompletionSource;
    private BehaviorSubject<double> progressSubject;

    public Playback(string fileName)
    {
      taskCompletionSource = new TaskCompletionSource<Unit>();

      progressSubject = new BehaviorSubject<double>(double.NaN);
      Progress = progressSubject;

      waveOut = new WaveOut();
      audioFileReader = new ProgressReportingAudioFileReader(fileName);
      audioFileReader.ProgressChanged += HandleProgressChanged;
      waveOut.Init(audioFileReader);
      waveOut.PlaybackStopped += HandlePlaybackStopped;
      
      waveOut.Play();
    }

    private void HandleProgressChanged(object sender, ProgressReportingAudioFileReader.ProgressChangedEventArgs e)
    {
      progressSubject.OnNext(e.Progress);
    }

    private void HandlePlaybackStopped(object sender, StoppedEventArgs e)
    {
      waveOut.Dispose();
      audioFileReader.Dispose();

      if (e.Exception == null)
      {
        taskCompletionSource.SetResult(Unit.Default);
        progressSubject.OnCompleted();
      }
      else
      {
        taskCompletionSource.SetException(e.Exception);
        progressSubject.OnError(e.Exception);
      }
    }

    #region Implementation of IPlayback

    public TaskAwaiter GetAwaiter()
    {
      return ((Task)taskCompletionSource.Task).GetAwaiter();
    }

    public void Stop()
    {
      waveOut.Stop();
    }

    public IObservable<double> Progress { get; }

    #endregion

    private class ProgressReportingAudioFileReader : AudioFileReader
    {
      public ProgressReportingAudioFileReader(string fileName) : base(fileName) {}

      #region Overrides of AudioFileReader

      public override int Read(byte[] buffer, int offset, int count)
      {
        var read = base.Read(buffer, offset, count);
        OnProgressChanged(new ProgressChangedEventArgs
                          {
                            Progress = (double) Position / (double) Length
                          });
        return read;
      }

      #endregion

      public struct ProgressChangedEventArgs
      {
        public double Progress;
      }

      public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

      protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
      {
        ProgressChanged?.Invoke(this, e);
      }
    }
  }
}