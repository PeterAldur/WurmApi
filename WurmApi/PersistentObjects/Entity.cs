using Newtonsoft.Json;

namespace AldursLab.WurmApi.PersistentObjects
{
    public abstract class Entity
    {
        [JsonProperty]
        public string ObjectId { get; internal set; }
        [JsonProperty]
        public int Version { get; internal set; }
    }
}