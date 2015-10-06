using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public class LogsMonitorEventArgs : EventArgs
    {
        public LogsMonitorEventArgs(CharacterName characterName, LogType logType, IEnumerable<LogEntry> wurmLogEntries, string pmRecipientNormalized = null)
        {
            if (characterName == null)
            {
                throw new ArgumentNullException("characterName");
            }
            if (wurmLogEntries == null)
            {
                throw new ArgumentNullException("wurmLogEntries");
            }
            if (pmRecipientNormalized == null) pmRecipientNormalized = string.Empty;

            this.CharacterName = characterName;
            this.LogType = logType;
            this.WurmLogEntries = wurmLogEntries;
            this.PmRecipientNormalized = pmRecipientNormalized;
        }

        public CharacterName CharacterName { get; private set; }
        public LogType LogType { get; private set; }
        public IEnumerable<LogEntry> WurmLogEntries { get; private set; }

        /// <summary>
        /// Optional. Available only in the context of PM logs. 
        /// Represents character name of the person, that currently monitored characted converses with.
        /// Name is normalized to UPPERCASE.
        /// Otherwise string.Empty
        /// </summary>
        public string PmRecipientNormalized { get; private set; }

        /// <summary>
        /// Converts PmRecipientNormalized to a representation, as it appears in the game.
        /// </summary>
        public string PmRecipientCapitalized
        {
            get
            {
                return CharacterName.UnnormalizeCharacterName(PmRecipientNormalized);
            }
        }
    }
}