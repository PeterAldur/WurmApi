using System;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Provides an API to work with many Wurm Online related data and functions.
    /// See remarks for more details.
    /// </summary>
    public interface IWurmApi : IDisposable
    {
        /// <summary>
        /// API that allows to interact and edit wurm client autorun files.
        /// </summary>
        IWurmAutoruns Autoruns { get; }
        /// <summary>
        /// API that allows to obtain information about state of wurm characters.
        /// </summary>
        IWurmCharacters Characters { get; }
        /// <summary>
        /// API that allows to read and edit wurm client configs.
        /// </summary>
        IWurmConfigs Configs { get; }
        /// <summary>
        /// API that defines game log types supported by WurmApi.
        /// </summary>
        IWurmLogDefinitions LogDefinitions { get; }
        /// <summary>
        /// API that allows searching through game log files.
        /// </summary>
        IWurmLogsHistory LogsHistory { get; }
        /// <summary>
        /// API that monitors game log files in real time.
        /// </summary>
        IWurmLogsMonitor LogsMonitor { get; }
        /// <summary>
        /// API that provides information about wurm servers.
        /// </summary>
        IWurmServers Servers { get; }
    }
}
