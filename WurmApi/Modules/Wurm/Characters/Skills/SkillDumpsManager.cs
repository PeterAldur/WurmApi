using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class SkillDumpsManager
    {
        readonly IWurmCharacter character;
        readonly IWurmApiLogger logger;
        readonly DirectoryInfo skillDumpsDirectory;
        readonly Dictionary<ServerGroupId, SkillDump> latestSkillDumps = new Dictionary<ServerGroupId, SkillDump>();

        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

        static readonly TimeSpan MaxDaysBack = TimeSpan.FromDays(336);

        public SkillDumpsManager([NotNull] IWurmCharacter character, [NotNull] IWurmPaths wurmPaths, IWurmApiLogger logger)
        {
            if (character == null) throw new ArgumentNullException("character");
            if (wurmPaths == null) throw new ArgumentNullException("wurmPaths");
            this.character = character;
            this.logger = logger;

            skillDumpsDirectory = new DirectoryInfo(wurmPaths.GetSkillDumpsFullPathForCharacter(character.Name));
        }

        public async Task<SkillDump> TryGetSkillDumpAsync(ServerGroupId serverGroupId)
        {
            SkillDump dump;
            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                if (!latestSkillDumps.TryGetValue(serverGroupId, out dump))
                {
                    await FindLatestDumpForServerGroup(serverGroupId).ConfigureAwait(false);
                    latestSkillDumps.TryGetValue(serverGroupId, out dump);
                }
            }
            finally
            {
                semaphore.Release();
            }

            return dump;
        }

        async Task FindLatestDumpForServerGroup(ServerGroupId serverGroupId)
        {
            var maxBackDate = Time.Get.LocalNow - MaxDaysBack;
            SkillDumpInfo[] dumps = new SkillDumpInfo[0];
            if (skillDumpsDirectory.Exists)
            {
                dumps = skillDumpsDirectory.GetFiles().Select(ConvertFileInfoToSkillDumpInfo)
                                           .Where(info => info != null && info.Stamp > maxBackDate)
                                           .OrderByDescending(info => info.Stamp)
                                           .ToArray();
            }

            SkillDump foundDump = null;
            foreach (var dumpInfo in dumps)
            {
                var server = await character.GetHistoricServerAtLogStampAsync(dumpInfo.Stamp).ConfigureAwait(false);
                if (server.ServerGroup.ServerGroupId == serverGroupId)
                {
                    foundDump = new RealSkillDump(serverGroupId, dumpInfo, logger);
                    break;
                }
            }

            if (foundDump != null)
            {
                latestSkillDumps[serverGroupId] = foundDump;
            }
            else
            {
                // if nothing found, place a stub to prevent another file search
                latestSkillDumps[serverGroupId] = new StubSkillDump(serverGroupId);
            }
        }

        SkillDumpInfo ConvertFileInfoToSkillDumpInfo(FileInfo fileInfo)
        {
            var stamp = TryParseDumpStamp(fileInfo);
            if (stamp == null)
            {
                return null;
            }
            return new SkillDumpInfo()
            {
                FileInfo = fileInfo,
                Stamp = stamp.Value
            };
        }

        DateTime? TryParseDumpStamp(FileInfo info)
        {
            try
            {

                var match = Regex.Match(info.Name,
                    @"skills\.(\d\d\d\d)(\d\d)(\d\d)\.(\d\d)(\d\d)\.txt",
                    RegexOptions.Compiled | RegexOptions.CultureInvariant);
                if (match.Success)
                {
                    var year = int.Parse(match.Groups[1].Value);
                    var month = int.Parse(match.Groups[2].Value);
                    var day = int.Parse(match.Groups[3].Value);
                    var hour = int.Parse(match.Groups[4].Value);
                    var minute = int.Parse(match.Groups[5].Value);
                    return new DateTime(year, month, day, hour, minute, 0);
                }
                else
                {
                    logger.Log(LogLevel.Error,
                        "match failed during parsing skill dump file date, file name: " + info.FullName,
                        this,
                        null);
                    return null;
                }
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error,
                    "exception during parsing skill dump file date, file name: " + info.FullName,
                    this,
                    exception);
                return null;
            }
        }
    }
}