using System;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    /// <summary>
    /// Represents information parsed from skill gain line.
    /// </summary>
    public class SkillInfo
    {
        public SkillInfo(string name, float value, DateTime stamp, float? gain)
        {
            NameNormalized = WurmSkills.NormalizeSkillName(name);
            Value = value;
            Stamp = stamp;
            Gain = gain;
        }

        /// <summary>
        /// Name of the skill, normalized to uppercase.
        /// </summary>
        public string NameNormalized { get; private set; }

        /// <summary>
        /// Value of the skill.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Stamp of the source log line.
        /// </summary>
        public DateTime Stamp { get; private set; }

        /// <summary>
        /// Skill gain value, if parsed.
        /// </summary>
        public float? Gain { get; private set; }
    }
}