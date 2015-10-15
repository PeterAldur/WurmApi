using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    abstract class SkillDump
    {
        protected SkillDump(ServerGroupId serverGroupId)
        {
            this.ServerGroupId = serverGroupId;
        }

        public ServerGroupId ServerGroupId { get; private set; }

        public abstract float? TryGetSkillLevel(string skillName);

        public abstract DateTime Stamp { get; }
    }

    class RealSkillDump : SkillDump
    {
        readonly SkillDumpInfo dumpInfo;
        readonly IReadOnlyDictionary<string, float> skillLevels;
        readonly IWurmApiLogger logger;

        public RealSkillDump(ServerGroupId serverGroupId, [NotNull] SkillDumpInfo dumpInfo,
            [NotNull] IWurmApiLogger logger)
            : base(serverGroupId)
        {
            if (dumpInfo == null) throw new ArgumentNullException("dumpInfo");
            if (logger == null) throw new ArgumentNullException("logger");
            this.dumpInfo = dumpInfo;
            this.logger = logger;
            skillLevels = ParseDump();
        }

        Dictionary<string, float> ParseDump()
        {
            var fileLines = File.ReadAllLines(dumpInfo.FileInfo.FullName);
            Dictionary<string, float> skills = new Dictionary<string, float>();
            var parser = new SkillEntryParser(logger);
            foreach (var line in fileLines)
            {
                if (line.StartsWith("Skills") 
                    || line.StartsWith("Characteristics") 
                    || line.StartsWith("Religion") 
                    || line.StartsWith("-----"))
                {
                    continue;
                }
                var match = Regex.Match(line, @"(.+): (.+) .+ .+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
                var skillName = match.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(skillName))
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Unparseable skill name in dump file {0}, raw line: {1}",
                            dumpInfo.FileInfo.FullName,
                            line),
                        this,
                        null);
                    continue;
                }
                var level = parser.TryParseFloatInvariant(match.Groups[2].Value);
                if (level == null)
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Unparseable skill value in dump file {0}, raw line: {1}",
                            dumpInfo.FileInfo.FullName,
                            line),
                        this,
                        null);
                    continue;
                }
                skills[WurmSkills.NormalizeSkillName(skillName)] = level.Value;
            }
            return skills;
        }

        public override float? TryGetSkillLevel(string skillName)
        {
            float value;
            if (skillLevels.TryGetValue(WurmSkills.NormalizeSkillName(skillName), out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public override DateTime Stamp
        {
            get { return dumpInfo.Stamp; }
        }
    }

    class StubSkillDump : SkillDump
    {
        public StubSkillDump(ServerGroupId serverGroupId) : base(serverGroupId)
        {
        }

        public override float? TryGetSkillLevel(string skillName)
        {
            return null;
        }

        public override DateTime Stamp
        {
            get { return DateTime.MinValue; }
        }
    }
}