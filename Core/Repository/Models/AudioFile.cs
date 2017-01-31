using Newtonsoft.Json;

namespace Core.Repository.Models
{
    public class AudioFile 
    {
        public string FullPath { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public LoadStatus LoadStatus { get; set; } = LoadStatus.Unknown;
    }
}