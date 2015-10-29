using System;
using System.Text.RegularExpressions;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.PersistentModel;

namespace AldursLab.WurmApi
{
    /// <summary>
    /// Contains extensions enabling parsing common information from log entries.
    /// </summary>
    public static class LogEntryParsingHelper
    {
        /// <summary>
        /// Attempts to parse server name from a log entry.
        /// Null if parsing failed.
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="logger">Optional, will log parsing errors.</param>
        /// <param name="sourceCharacterName">Optional, will be appended to log message.</param>
        /// <returns></returns>
        public static ServerStamp TryGetServerFromLogEntry(this LogEntry logEntry, IWurmApiLogger logger = null, CharacterName sourceCharacterName = null)
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
                    if (logger != null)
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
            }
            return result;
        }
    }
}
