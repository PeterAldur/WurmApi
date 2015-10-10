using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Events.Public;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class WurmCharacterSkills : IWurmCharacterSkills, IHandle<YouAreOnEventDetectedOnLiveLogs>
    {
        readonly IWurmCharacter character;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IWurmLogsMonitorInternal logsMonitor;
        readonly IWurmLogsHistory logsHistory;
        readonly IWurmApiLogger logger;

        readonly SkillsMap skillsMap;
        readonly SkillDumpsManager skillDumps;

        IWurmServer currentServer;

        readonly TaskCompletionSource<DateTime> currentServerLookupFinished = new TaskCompletionSource<DateTime>();
        DateTime? scannedMinDate;

        readonly SemaphoreSlim scanJobSemaphore = new SemaphoreSlim(1,1);

        public event EventHandler<SkillsChangedEventArgs> SkillsChanged;

        readonly ConcurrentBag<string> changedSkills = new ConcurrentBag<string>();

        readonly PublicEvent onSkillsChanged;

        public WurmCharacterSkills([NotNull] IWurmCharacter character, [NotNull] IPublicEventInvoker publicEventInvoker,
            [NotNull] IWurmLogsMonitorInternal logsMonitor, [NotNull] IWurmLogsHistory logsHistory,
            [NotNull] IWurmApiLogger logger, IWurmPaths wurmPaths,
            [NotNull] IInternalEventAggregator internalEventAggregator)
        {
            if (character == null) throw new ArgumentNullException("character");
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            if (logsMonitor == null) throw new ArgumentNullException("logsMonitor");
            if (logsHistory == null) throw new ArgumentNullException("logsHistory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (internalEventAggregator == null) throw new ArgumentNullException("internalEventAggregator");
            this.character = character;
            this.publicEventInvoker = publicEventInvoker;
            this.logsMonitor = logsMonitor;
            this.logsHistory = logsHistory;
            this.logger = logger;

            skillsMap = new SkillsMap();
            skillDumps = new SkillDumpsManager(character, wurmPaths, logger);

            UpdateCurrentServer();

            onSkillsChanged =
                publicEventInvoker.Create(
                    InvokeOnSkillsChanged,
                    WurmApiTuningParams.PublicEventMarshallerDelay);

            internalEventAggregator.Subscribe(this);

            logsMonitor.SubscribeInternal(character.Name, LogType.Skills, EventHandler);
        }

        void InvokeOnSkillsChanged()
        {
            SkillsChanged.SafeInvoke(this, new SkillsChangedEventArgs(TakeChangedSkills()));
        }

        string[] TakeChangedSkills()
        {
            List<string> skills = new List<string>();
            string skill;
            while (changedSkills.TryTake(out skill))
            {
                skills.Add(skill);
            }
            return skills.Distinct().ToArray();
        }

        async void UpdateCurrentServer()
        {
            try
            {
                currentServer = await character.GetCurrentServerAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "error on updating current server", this, exception);
            }
            currentServerLookupFinished.TrySetResult(Time.Get.LocalNow);
        }

        void EventHandler(object sender, LogsMonitorEventArgs logsMonitorEventArgs)
        {
            // note: event needs to be triggered regardless of skill being already known and up to date

            // there is no point in updating skill values, if server is not known
            //if (currentServer == null) return;

            SkillEntryParser parser = new SkillEntryParser(logger);
            bool anyParsed = false;
            foreach (var wurmLogEntry in logsMonitorEventArgs.WurmLogEntries)
            {
                SkillInfo skillInfo = parser.TryParseSkillInfoFromLogLine(wurmLogEntry);
                if (skillInfo != null)
                {
                    changedSkills.Add(skillInfo.NameNormalized);
                    skillsMap.UpdateSkill(skillInfo, currentServer);
                    anyParsed = true;
                }
            }

            if (anyParsed) onSkillsChanged.Trigger();
        }

        public async Task<float?> TryGetCurrentSkillLevelAsync(string skillName, ServerGroupId serverGroupId,
            TimeSpan maxTimeToLookBackInLogs)
        {
            // note: semaphore(1,1) in this method ensures, that there are no races
            // be extra careful if loosening this constraint!

            try
            {
                await scanJobSemaphore.WaitAsync().ConfigureAwait(false);
                await ScanLogsHistory(maxTimeToLookBackInLogs).ConfigureAwait(false);
                var skill = skillsMap.TryGetSkill(skillName, serverGroupId);
                if (skill == null)
                {
                    // as a final option, try to use skill dumps, if available
                    var dump = await skillDumps.TryGetSkillDumpAsync(serverGroupId).ConfigureAwait(false);
                    if (dump != null)
                    {
                        skill = dump.TryGetSkillLevel(skillName);
                    }
                }
                return skill;
            }
            finally
            {
                scanJobSemaphore.Release();
            }
        }

        private async Task ScanLogsHistory(TimeSpan maxTimeToLookBackInLogs)
        {
            DateTime maxDate = await currentServerLookupFinished.Task.ConfigureAwait(false);
            DateTime minDate = maxDate.SubtractConstrain(maxTimeToLookBackInLogs);

            // if already scanned, optimize
            if (scannedMinDate != null)
            {
                if (minDate >= scannedMinDate)
                {
                    // do not scan, if this period has already been scanned
                    return;
                }
                maxDate = scannedMinDate.Value;
            }

            var entries = await logsHistory.ScanAsync(new LogSearchParameters()
            {
                CharacterName = character.Name.Normalized,
                LogType = LogType.Skills,
                MinDate = minDate,
                MaxDate = maxDate
            }).ConfigureAwait(false);
            scannedMinDate = minDate;
            SkillEntryParser parser = new SkillEntryParser(logger);
            foreach (var wurmLogEntry in entries)
            {
                SkillInfo skillInfo = parser.TryParseSkillInfoFromLogLine(wurmLogEntry);
                if (skillInfo != null)
                {
                    var entryServer = await character.GetHistoricServerAtLogStampAsync(wurmLogEntry.Timestamp).ConfigureAwait(false);
                    skillsMap.UpdateSkill(skillInfo, entryServer);
                }
            }
        }

        public void Handle(YouAreOnEventDetectedOnLiveLogs message)
        {
            if (message.CharacterName == character.Name && message.CurrentServerNameChanged)
            {
                UpdateCurrentServer();
            }
        }
    }
}
