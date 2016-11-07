using System;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using NAudio.Wave;

namespace Core.Audio
{
  [Export(typeof(IAudioService))]
  public class AudioService : IAudioService
  {
    [ImportingConstructor]
    public AudioService()
    {
    }

    #region Implementation of IAudioService

    public void PlayAudioFile(string path)
    {
      var device = new WaveOut();
      var reader = new AudioFileReader(path);
      device.PlaybackStopped += (_,__) => device.Dispose();
      device.Init(reader);
      device.Play();
    }

    #endregion

  }
}