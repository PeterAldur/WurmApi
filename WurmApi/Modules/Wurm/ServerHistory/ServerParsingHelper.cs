using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    public static class ServerParsingHelper
    {
        internal static ServerStamp TryGetServerFromLogEntry(LogEntry logEntry, IWurmApiLogger logger)
        {
            ServerStamp result = null;
            if (Regex.IsMatch(logEntry.Content, @"^\d+ other players", RegexOptions.Compiled))
            {
                Match match = Regex.Match(
                    logEntry.Content,
                    @"\d+ other players are online.*\. You are on (.+) \(",
                    RegexOptions.Compiled);
                if (match.Success)
                {
                    var serverName = new ServerName(match.Groups[1].Value.ToUpperInvariant());
                    result = new ServerStamp() {ServerName = serverName, Timestamp = logEntry.Timestamp};
                }
                else
                {
                    logger.Log(
                        LogLevel.Warn,
                        "ServerHistoryProvider found 'you are on' log line, but could not parse it. Entry: "
                        + logEntry,
                        "ServerParsingHelper",
                        null);
                }
            }
            return result;
        }
    }
}
