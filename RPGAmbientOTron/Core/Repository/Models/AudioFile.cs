namespace Core.Repository.Models
{
    public class AudioFile 
    {
        public string FullPath { get; set; }

        public string Name { get; set; }

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