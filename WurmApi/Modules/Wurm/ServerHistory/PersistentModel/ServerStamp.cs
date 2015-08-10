using System;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel
{
    public class ServerStamp
    {
        public DateTime Timestamp { get; set; }
        public ServerName ServerName { get; set; }
    }
}