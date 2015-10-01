using System;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class LogHistory
    {
        readonly IWurmLogsHistory wurmLogsHistory;
        readonly IWurmCharacterDirectories wurmCharacterDirectories;
        readonly IWurmServerHistory wurmServerHistory;
        readonly LogHistorySaved logHistorySaved;
        readonly LogEntriesParser parser;
        readonly IWurmApiLogger wurmApiLogger;

        bool scanned = false;

        public LogHistory(
            IWurmLogsHistory wurmLogsHistory,
            IWurmCharacterDirectories wurmCharacterDirectories,
            IWurmServerHistory wurmServerHistory,
            LogHistorySaved logHistorySaved,
            LogEntriesParser parser, 
            [NotNull] IWurmApiLogger wurmApiLogger)
        {
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logHistorySaved == null) throw new ArgumentNullException("logHistorySaved");
            if (parser == null) throw new ArgumentNullException("parser");
            if (wurmApiLogger == null) throw new ArgumentNullException("wurmApiLogger");
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.wurmServerHistory = wurmServerHistory;
            this.logHistorySaved = logHistorySaved;
            this.parser = parser;
            this.wurmApiLogger = wurmApiLogger;
        }

        public TimeDetails GetForServer(ServerName serverName)
        {
            EnsureScanned();

            logHistorySaved.LastScanDate = Time.Get.LocalNowOffset;

            return logHistorySaved.GetHistoricForServer(serverName);
        }

        void EnsureScanned()
        {
            if (scanned) return;

            var maxScanSince = Time.Get.LocalNowOffset.AddDays(-30);
            var lastScanSince = logHistorySaved.LastScanDate.AddDays(-1);
            var scanSince = lastScanSince < maxScanSince ? maxScanSince : lastScanSince;

            var allChars = wurmCharacterDirectories.GetAllCharacters();
            foreach (var characterName in allChars)
            {
                var searchResults = wurmLogsHistory.Scan(
                    new LogSearchParameters()
                    {
                        CharacterName = characterName,
                        DateFrom = scanSince.DateTime,
                        DateTo = Time.Get.LocalNow,
                        LogType = LogType.Event
                    });
                foreach (var searchResult in searchResults)
                {
                    var upt = parser.TryParseUptime(searchResult);
                    if (upt != null)
                    {
                        try
                        {
                            var server = wurmServerHistory.GetServer(characterName, searchResult.Timestamp);
                            logHistorySaved.UpdateHistoric(server, upt);
                        }
                        catch (DataNotFoundException)
                        {
                            wurmApiLogger.Log(LogLevel.Info,
                                string.Format("Server not found for character {0} at timestamp {1}",
                                    characterName,
                                    searchResult.Timestamp),
                                this,
                                null);
                        }

                    }
                    var wdt = parser.TryParseWurmDateTime(searchResult);
                    if (wdt != null)
                    {
                        try
                        {
                            var server = wurmServerHistory.GetServer(characterName, searchResult.Timestamp);
                            logHistorySaved.UpdateHistoric(server, wdt);
                        }
                        catch (DataNotFoundException)
                        {
                            wurmApiLogger.Log(LogLevel.Info,
                                string.Format("Server not found for character {0} at timestamp {1}",
                                    characterName,
                                    searchResult.Timestamp),
                                this,
                                null);
                        }
                    }
                }
            }

            scanned = true;
        }
    }
}