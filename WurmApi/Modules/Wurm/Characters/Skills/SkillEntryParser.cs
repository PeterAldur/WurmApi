using System;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class SkillEntryParser
    {
        readonly IWurmApiLogger logger;

        public SkillEntryParser([NotNull] IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public SkillInfo TryParseSkillInfoFromLogLine(LogEntry wurmLogEntry)
        {
            if (wurmLogEntry.Content.Contains("increased") | wurmLogEntry.Content.Contains("decreased"))
            {
                var match = Regex.Match(wurmLogEntry.Content,
                    @"^(.+) (?:increased|decreased) by.* to (\d+(?:\,|\.)\d+|\d+).*$",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                string skillName = null;
                float? parsedLevel = null;
                if (match.Success)
                {
                    skillName = match.Groups[1].Value;

                    if (string.IsNullOrWhiteSpace(skillName))
                    {
                        logger.Log(LogLevel.Error,
                            "Skill name was parsed to empty string, raw entry: " + wurmLogEntry,
                            this,
                            null);
                        return null;
                    }

                    var rawSkillLevel = match.Groups[2].Value;

                    parsedLevel = TryParseFloatInvariant(rawSkillLevel);

                    if (parsedLevel == null)
                    {
                        logger.Log(LogLevel.Error,
                            "Skill level could not be parsed, raw value: " + rawSkillLevel
                            + " raw entry: " + wurmLogEntry,
                            this,
                            null);
                        return null;
                    }
                }
                else
                {
                    logger.Log(LogLevel.Error,
                        "Skill gain/loss message could not be parsed, raw entry: " + wurmLogEntry,
                        this,
                        null);
                    return null;
                }

                return new SkillInfo(skillName, parsedLevel.Value, wurmLogEntry.Timestamp);
            }
            else
            {
                return null;
            }
        }

        public float? TryParseFloatInvariant(string text)
        {
            float? parsedLevel = null;
            float level = -1;
            if (float.TryParse(
                text,
                System.Globalization.NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out level))
            {
                parsedLevel = level;
            }
            else if (float.TryParse(
                text.Replace(",", "."),
                System.Globalization.NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out level))
            {
                parsedLevel = level;
            }

            return parsedLevel;
        }
    }
}