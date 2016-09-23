namespace Core.Repository.Models
{
    public class AudioFile 
    {
        public Core.Persistence.Models.AudioFile PersistenceModel { get; set; }

        public byte[] Hash { get; set; }

        public LoadStatus LoadStatus { get; set; } = LoadStatus.Unknown;
    }

    public enum LoadStatus
    {
        Unknown,
        FileNotFound,
        LoadError,
        FileOk
    }
}