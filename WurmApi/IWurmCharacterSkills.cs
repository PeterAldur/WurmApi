using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmApi
{
    public interface IWurmCharacterSkills
    {
        /// <summary>
        /// Attempts to find out current level of a given skill.
        /// Combined data of live logs, logs history and skill dumps is used to obtain this information.
        /// </summary>
        /// <param name="skillName">Name of the skill, case insensitive.</param>
        /// <param name="serverGroup"></param>
        /// <param name="maxTimeToLookBackInLogs">Maximum number of days to scan logs history, before giving up.</param>
        /// <returns></returns>
        Task<float?> TryGetCurrentSkillLevelAsync(string skillName, ServerGroup serverGroup, TimeSpan maxTimeToLookBackInLogs);

        /// <summary>
        /// Attempts to find out current level of a given skill.
        /// Combined data of live logs, logs history and skill dumps is used to obtain this information.
        /// </summary>
        /// <param name="skillName">Name of the skill, case insensitive.</param>
        /// <param name="serverGroup"></param>
        /// <param name="maxTimeToLookBackInLogs">Maximum number of days to scan logs history, before giving up.</param>
        /// <returns></returns>
        float? TryGetCurrentSkillLevel(string skillName, ServerGroup serverGroup, TimeSpan maxTimeToLookBackInLogs);

        /// <summary>
        /// Triggered, when some skills have changed since last invocation of this event.
        /// Only live logs feed is being monitored.
        /// </summary>
        event EventHandler<SkillsChangedEventArgs> SkillsChanged;
    }

    public class SkillsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Skills, that changed since last invocation of this event.
        /// Names of the skills are always normalized to uppercase.
        /// </summary>
        public IReadOnlyList<string> ChangedSkills { get; private set; }


        public SkillsChangedEventArgs(string[] changedSkills)
        {
            ChangedSkills = changedSkills;
        }

        /// <summary>
        /// True if skill has changed.
        /// </summary>
        /// <param name="skillName">Case insensitive.</param>
        /// <returns></returns>
        public bool HasSkillChanged(string skillName)
        {
            skillName = skillName.ToUpperInvariant();
            return ChangedSkills.Any(s => s.Equals(skillName));
        }
    }
}
