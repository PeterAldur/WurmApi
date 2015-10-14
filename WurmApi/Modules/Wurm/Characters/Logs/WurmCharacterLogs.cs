using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Logs
{
    class WurmCharacterLogs : IWurmCharacterLogs
    {
        readonly IWurmCharacter character;
        readonly IWurmServerGroups serverGroups;
        readonly IWurmLogsHistory logsHistory;
        readonly IWurmServers wurmServers;
        readonly IWurmApiLogger logger;

        public WurmCharacterLogs(
            [NotNull] IWurmCharacter character, 
            [NotNull] IWurmServerGroups serverGroups,
            [NotNull] IWurmLogsHistory logsHistory,
            [NotNull] IWurmServers wurmServers, 
            [NotNull] IWurmApiLogger logger)
        {
            if (character == null) throw new ArgumentNullException("character");
            if (serverGroups == null) throw new ArgumentNullException("serverGroups");
            if (logsHistory == null) throw new ArgumentNullException("logsHistory");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (logger == null) throw new ArgumentNullException("logger");
            this.character = character;
            this.serverGroups = serverGroups;
            this.logsHistory = logsHistory;
            this.wurmServers = wurmServers;
            this.logger = logger;
        }

        public async Task<IList<LogEntry>> ScanLogsAsync(DateTime minDate, DateTime maxDate, LogType logType)
        {
            return await logsHistory.ScanAsync(new LogSearchParameters()
            {
                CharacterName = character.Name.Normalized,
                LogType = logType,
                MinDate = minDate,
                MaxDate = maxDate
            }).ConfigureAwait(false);
        }

        public async Task<IList<LogEntry>> ScanLogsServerGroupRestrictedAsync(DateTime minDate, DateTime maxDate, LogType logType,
            ServerGroupId serverGroupId)
        {
            if (serverGroups.All.All(@group => @group.ServerGroupId != serverGroupId))
            {
                throw new InvalidSearchParametersException("Unknown enum value for ServerGroupId");
            }

            var results = await logsHistory.ScanAsync(new LogSearchParameters()
            {
                CharacterName = character.Name.Normalized,
                LogType = logType,
                MinDate = minDate,
                MaxDate = maxDate
            }).ConfigureAwait(false);

            List<LogEntry> filteredEntries = new List<LogEntry>();
            foreach (var logEntry in results)
            {
                var serverForEntry = await TryGetServerAtStamp(logEntry.Timestamp).ConfigureAwait(false);
                if (serverForEntry != null && serverForEntry.ServerGroup.ServerGroupId == serverGroupId)
                {
                    filteredEntries.Add(logEntry);
                }
            }
            return filteredEntries;
        }

        async Task<IWurmServer> TryGetServerAtStamp(DateTime dateTime)
        {
            var result = await character.TryGetHistoricServerAtLogStampAsync(dateTime).ConfigureAwait(false);
            if (result == null)
            {
                logger.Log(LogLevel.Info,
                    string.Format("Server could not be identified for character {0} at stamp {1}",
                        character.Name.Capitalized,
                        dateTime),
                    this,
                    null);
                return null;
            }
            return result;
        }
    }
}