using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.Jobs;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    class WurmServerHistory : IWurmServerHistory
    {
        readonly IWurmLogsHistory wurmLogsHistory;
        readonly IWurmServerGroups wurmServerGroups;
        readonly QueuedJobsSyncRunner<object, ServerName> runner;
        JobExecutor jobExecutor;

        public WurmServerHistory(
            [NotNull] string dataDirectoryFullPath, 
            [NotNull] IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            IWurmApiLogger logger,
            IWurmLogsMonitorInternal wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles,
            IInternalEventAggregator internalEventAggregator, 
            [NotNull] IWurmServerGroups wurmServerGroups)
        {
            if (dataDirectoryFullPath == null) throw new ArgumentNullException("dataDirectoryFullPath");
            if (wurmLogsHistory == null) throw new ArgumentNullException("wurmLogsHistory");
            if (wurmServerGroups == null) throw new ArgumentNullException("wurmServerGroups");
            this.wurmLogsHistory = wurmLogsHistory;
            this.wurmServerGroups = wurmServerGroups;
            var persistentLibrary =
                new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(dataDirectoryFullPath),
                    new PersObjErrorHandlingStrategy(logger));
            var collection = persistentLibrary.GetCollection("serverhistory");

            var providerFactory = new ServerHistoryProviderFactory(
                collection,
                wurmLogsHistory,
                wurmServerList,
                logger,
                wurmLogsMonitor,
                wurmLogFiles,
                internalEventAggregator);


            jobExecutor = new JobExecutor(providerFactory, persistentLibrary);
            runner = new QueuedJobsSyncRunner<object, ServerName>(jobExecutor, logger);
        }

        public async Task<ServerName> TryGetServerAsync(CharacterName character, DateTime exactDate)
        {
            return await TryGetServerAsync(character, exactDate, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ServerName> TryGetServerAsync(CharacterName character, DateTime exactDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // attempt to check cache directly, result might have been already found by another query.
            var fastResult = jobExecutor.CheckCacheForServerInfo(character, exactDate);
            if (fastResult != null)
            {
                return fastResult;
            }
            return await runner.Run(new GetServerAtDateJob(character, exactDate), cancellationToken).ConfigureAwait(false);
        }

        public ServerName TryGetServer(CharacterName character, DateTime exactDate)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetServerAsync(character, exactDate).Result);
        }

        public ServerName TryGetServer(CharacterName character, DateTime exactDate, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetServerAsync(character, exactDate, cancellationToken).Result);
        }

        public async Task<ServerName> TryGetCurrentServerAsync(CharacterName character)
        {
            return await TryGetCurrentServerAsync(character, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ServerName> TryGetCurrentServerAsync(CharacterName character, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await runner.Run(new GetCurrentServerJob(character), cancellationToken).ConfigureAwait(false);
        }

        public ServerName TryGetCurrentServer(CharacterName character)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentServerAsync(character).Result);
        }

        public ServerName TryGetCurrentServer(CharacterName character, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentServerAsync(character, cancellationToken).Result);
        }

        public void BeginTracking(CharacterName name)
        {
            jobExecutor.BeginTrackingForCharacter(name);
        }
    }
}
