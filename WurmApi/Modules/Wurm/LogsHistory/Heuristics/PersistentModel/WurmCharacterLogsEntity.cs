using System.Collections.Generic;
using AldursLab.WurmApi.PersistentObjects;
using Newtonsoft.Json;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel
{
    [JsonObject(MemberSerialization.Fields)]
    class WurmCharacterLogsEntity : Entity
    {
        /// <summary>
        /// Key: file name normalized, Value: file information
        /// </summary>
        public Dictionary<string, WurmLogMonthlyFile> WurmLogFiles { get; } = new Dictionary<string, WurmLogMonthlyFile>();
    }
}