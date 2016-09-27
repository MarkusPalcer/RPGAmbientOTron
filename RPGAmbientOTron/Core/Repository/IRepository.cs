using Core.Repository.Models;

namespace Core.Repository
{
    public interface IRepository
    {
        AudioFile GetAudioFileModel(string fileName);
    }
}