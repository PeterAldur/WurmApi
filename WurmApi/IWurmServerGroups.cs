using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public interface IWurmServerGroups
    {
        /// <summary>
        /// Returns all server groups, that currently exist in Wurm.
        /// </summary>
        IEnumerable<ServerGroup> AllValid { get; }

        ServerGroup Get(ServerGroupId serverGroupId);
    }
}
