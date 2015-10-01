using System;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using AldursLab.WurmApi.Modules.Wurm.ServerHistory.Jobs;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.ServerHistory
{
    class WurmServerHistory : IWurmServerHistory
    {
        readonly QueuedJobsSyncRunner<object, ServerName> runner;

        public WurmServerHistory(
            string dataDirectoryFullPath,
            IWurmLogsHistory wurmLogsHistory,
            IWurmServerList wurmServerList,
            IWurmApiLogger logger,
            IWurmLogsMonitorInternal wurmLogsMonitor,
            IWurmLogFiles wurmLogFiles)
        {
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
                wurmLogFiles);

            runner = new QueuedJobsSyncRunner<object, ServerName>(new JobExecutor(providerFactory, persistentLibrary), logger);
        }

        public async Task<ServerName> GetServerAsync(CharacterName character, DateTime exactDate)
        {
            return await GetServerAsync(character, exactDate, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ServerName> GetServerAsync(CharacterName character, DateTime exactDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await runner.Run(new GetServerAtDateJob(character, exactDate), cancellationToken).ConfigureAwait(false);
        }

        public ServerName GetServer(CharacterName character, DateTime exactDate)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetServerAsync(character, exactDate).Result);
        }

        public ServerName GetServer(CharacterName character, DateTime exactDate, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetServerAsync(character, exactDate, cancellationToken).Result);
        }

        public async Task<ServerName> GetCurrentServerAsync(CharacterName character)
        {
            return await GetCurrentServerAsync(character, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<ServerName> GetCurrentServerAsync(CharacterName character, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await runner.Run(new GetCurrentServerJob(character), cancellationToken).ConfigureAwait(false);
        }

        public ServerName GetCurrentServer(CharacterName character)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetCurrentServerAsync(character).Result);
        }

        public ServerName GetCurrentServer(CharacterName character, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => GetCurrentServerAsync(character, cancellationToken).Result);
        }
    }
}
