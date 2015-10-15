using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;
using AldursLab.WurmApi.Extensions.DotNet;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    public static class ServerParsingHelper
    {
        internal static ServerStamp TryGetServerFromLogEntry(LogEntry logEntry, IWurmApiLogger logger, CharacterName sourceCharacterName = null)
        {
            ServerStamp result = null;
            // attempt some faster matchers first, before trying actual parse
            if (Regex.IsMatch(logEntry.Content, @"^\d+ other players", RegexOptions.Compiled)
                && logEntry.Content.Contains("You are on", StringComparison.InvariantCultureIgnoreCase))
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
                        string.Format(
                            "ServerHistoryProvider found 'you are on' log line, but could not parse it. Character: {0} Entry: {1}",
                            sourceCharacterName,
                            logEntry),
                        "ServerParsingHelper",
                        null);
                }
            }
            return result;
        }
    }
}
