using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Encapsulates Wurm Online server groups.
    /// </summary>
    public interface IWurmServerGroups
    {
        /// <summary>
        /// Returns all Wurm Online server groups defined in WurmApi.
        /// </summary>
        IEnumerable<ServerGroup> All { get; }

        /// <summary>
        /// Returns server group by it's Id, if exists.
        /// </summary>
        /// <param name="serverGroupId"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException">Id is outside defined enum values</exception>
        ServerGroup GetById(ServerGroupId serverGroupId);
    }
}
