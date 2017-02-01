using System.ComponentModel.Composition;
using Core.Repository.Models.Sources;

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

    public IPlayback PlayAudioFile(AudioFile path)
    {
      return new Playback(path.Open());
    }

    #endregion

  }
}