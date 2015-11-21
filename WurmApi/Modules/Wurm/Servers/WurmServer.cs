using System;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.Servers.Jobs;
using AldursLab.WurmApi.Modules.Wurm.Servers.WurmServersModel;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Servers
{
    class WurmServer : IWurmServer
    {
        private readonly WurmServerInfo wurmServerInfo;
        readonly QueuedJobsSyncRunner<Job, JobResult> jobRunner;
        readonly ServerGroup serverScopedServerGroup;

        internal WurmServer(WurmServerInfo wurmServerInfo, [NotNull] QueuedJobsSyncRunner<Job, JobResult> jobRunner)
        {
            if (wurmServerInfo == null) throw new ArgumentNullException("wurmServerInfo");
            if (jobRunner == null) throw new ArgumentNullException("jobRunner");
            this.wurmServerInfo = wurmServerInfo;
            this.jobRunner = jobRunner;
            serverScopedServerGroup = ServerGroup.CreateServerScoped(wurmServerInfo.ServerName);
        }

        public ServerName ServerName
        {
            get
            {
                return wurmServerInfo.ServerName;
            }
        }

        public ServerGroup ServerGroup
        {
            get
            {
                return wurmServerInfo.ServerGroup;
            }
        }

        [Obsolete]
        public ServerGroup ServerScopedServerGroup
        {
            get { return serverScopedServerGroup; }
        }

        #region TryGetCurrentTime

        public async Task<WurmDateTime?> TryGetCurrentTimeAsync()
        {
            return await TryGetCurrentTimeAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public WurmDateTime? TryGetCurrentTime()
        {
            return TryGetCurrentTime(CancellationToken.None);
        }

        public async Task<WurmDateTime?> TryGetCurrentTimeAsync(CancellationToken cancellationToken)
        {
            var result =
                await
                    jobRunner.Run(new CurrentWurmDateTimeJob(ServerName), cancellationToken).ConfigureAwait(false);
            return result.WurmDateTime;
        }

        public WurmDateTime? TryGetCurrentTime(CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentTimeAsync(cancellationToken).Result);
        }

        #endregion

        private WurmDateTime AdjustedWurmDateTime(ServerDateStamped date)
        {
            return date.WurmDateTime + (Time.Get.LocalNowOffset - date.Stamp);
        }

        #region TryGetCurrentUptime

        public async Task<TimeSpan?> TryGetCurrentUptimeAsync()
        {
            return await TryGetCurrentUptimeAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public TimeSpan? TryGetCurrentUptime()
        {
            return TryGetCurrentUptime(CancellationToken.None);
        }

        public async Task<TimeSpan?> TryGetCurrentUptimeAsync(CancellationToken cancellationToken)
        {
            var result =
                await
                    jobRunner.Run(new CurrentUptimeJob(ServerName), cancellationToken).ConfigureAwait(false);
            return result.Uptime;
        }

        public TimeSpan? TryGetCurrentUptime(CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => TryGetCurrentUptimeAsync(cancellationToken).Result);
        }

        #endregion

        private TimeSpan AdjustedUptime(ServerUptimeStamped uptime)
        {
            return uptime.Uptime + (Time.Get.LocalNowOffset - uptime.Stamp);
        }
    }
}