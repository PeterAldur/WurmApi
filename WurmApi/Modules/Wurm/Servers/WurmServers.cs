using System;
using System.Collections.Generic;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Modules.Wurm.ServerGroups;
using AldursLab.WurmApi.Modules.Wurm.Servers.Jobs;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    /// <summary>
    /// Manages information about wurm game servers.
    /// </summary>
    class WurmServers : IWurmServers, IDisposable
    {
        readonly Dictionary<ServerName, WurmServer> nameToServerMap = new Dictionary<ServerName, WurmServer>();

        readonly WurmServerFactory wurmServerFactory;
        readonly LiveLogsDataQueue liveLogsDataQueue;

        readonly QueuedJobsSyncRunner<Job, JobResult> runner;

        readonly PersistentCollectionsLibrary persistentCollectionsLibrary;

        public WurmServers(
            IWurmLogsHistory wurmLogsHistory,
            IWurmLogsMonitorInternal wurmLogsMonitor,
            IWurmServerList wurmServerList,
            IHttpWebRequests httpWebRequests,
            string dataDirectory,
            IWurmCharacterDirectories wurmCharacterDirectories,
            IWurmServerHistory wurmServerHistory,
            IWurmApiLogger logger)
        {
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmLogsMonitor == null) throw new ArgumentNullException("wurmLogsMonitor");
            if (wurmServerList == null) throw new ArgumentNullException("wurmServerList");
            if (httpWebRequests == null) throw new ArgumentNullException("httpWebRequests");
            if (dataDirectory == null) throw new ArgumentNullException("dataDirectory");
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");

            liveLogsDataQueue = new LiveLogsDataQueue(wurmLogsMonitor);
            LiveLogs liveLogs = new LiveLogs(liveLogsDataQueue, wurmServerHistory);

            persistentCollectionsLibrary =
                new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(dataDirectory),
                    new PersObjErrorHandlingStrategy(logger));
            var persistent = persistentCollectionsLibrary.DefaultCollection.GetObject<ServersData>("WurmServers");
            LogHistorySaved logHistorySaved = new LogHistorySaved(persistent);
            LogHistory logHistory = new LogHistory(wurmLogsHistory,
                wurmCharacterDirectories,
                wurmServerHistory,
                logHistorySaved,
                new LogEntriesParser(),
                logger);
            
            WebFeeds webFeeds = new WebFeeds(httpWebRequests, wurmServerList, logger);

            runner = new QueuedJobsSyncRunner<Job, JobResult>(new JobRunner(liveLogs, logHistory, webFeeds, persistentCollectionsLibrary), logger);

            wurmServerFactory = new WurmServerFactory(runner);

            var descriptions = wurmServerList.All;
            foreach (var serverDescription in descriptions)
            {
                RegisterServer(serverDescription);
            }
        }

        private WurmServer RegisterServer(WurmServerInfo wurmServerInfo)
        {
            var normalizedName = wurmServerInfo.Name;
            if (this.nameToServerMap.ContainsKey(normalizedName))
            {
                throw new WurmApiException("Server already registered: " + wurmServerInfo);
            }

            var server = wurmServerFactory.Create(wurmServerInfo);

            this.nameToServerMap.Add(normalizedName, server);

            return server;
        }

        public IEnumerable<IWurmServer> All
        {
            get
            {
                return this.nameToServerMap.Values;
            }
        }

        public IWurmServer GetByName(ServerName name)
        {
            if (name == null) throw new ArgumentNullException("name");

            WurmServer server;
            if (!this.nameToServerMap.TryGetValue(name, out server))
            {
                // registering unknown server
                return RegisterServer(new WurmServerInfo(name.Original, String.Empty, new UnknownServerGroup()));
            }
            return server;
        }

        public IWurmServer GetByName(string name)
        {
            return GetByName(new ServerName(name));
        }

        public void Dispose()
        {
            persistentCollectionsLibrary.SaveChanged();
            liveLogsDataQueue.Dispose();
            runner.Dispose();
        }
    }
}