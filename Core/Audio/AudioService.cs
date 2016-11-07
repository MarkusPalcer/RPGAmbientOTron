using System.ComponentModel.Composition;

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

    public IPlayback PlayAudioFile(string path)
    {
      return new Playback(path);
    }

    #endregion

  }
}