using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.Modules.Wurm.ServerGroups;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class WurmServerList : IWurmServerList
    {
        private readonly IReadOnlyCollection<WurmServerInfo> defaultDescriptions;

        public WurmServerList(IDictionary<ServerName, WurmServerInfo> serversMap)
        {
            defaultDescriptions = serversMap.Select(pair => pair.Value).ToArray();
        }

        public virtual IEnumerable<WurmServerInfo> All
        {
            get { return defaultDescriptions; }
        }

        public bool Exists(ServerName serverName)
        {
            return defaultDescriptions.Any(info => info.ServerName == serverName);
        }
    }
}