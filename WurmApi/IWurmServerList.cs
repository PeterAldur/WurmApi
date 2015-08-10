using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public interface IWurmServerList
    {
        IEnumerable<WurmServerInfo> All { get; }

        bool Exists(ServerName serverName);
    }
}