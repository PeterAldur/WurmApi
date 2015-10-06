using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Events.Internal.Messages
{
    /// <summary>
    /// This event happens when player logs in or travels between server (which is technically a login to another server as well)
    /// </summary>
    class YouAreOnEventDetectedOnLiveLogs : Message
    {
        readonly ServerName serverName;
        readonly CharacterName characterName;
        readonly bool currentServerNameChanged;

        public YouAreOnEventDetectedOnLiveLogs([NotNull] ServerName serverName, [NotNull] CharacterName characterName,
            bool currentServerNameChanged)
        {
            if (serverName == null) throw new ArgumentNullException("serverName");
            if (characterName == null) throw new ArgumentNullException("characterName");
            this.serverName = serverName;
            this.characterName = characterName;
            this.currentServerNameChanged = currentServerNameChanged;
        }

        public ServerName ServerName
        {
            get { return serverName; }
        }

        public CharacterName CharacterName
        {
            get { return characterName; }
        }

        public bool CurrentServerNameChanged
        {
            get { return currentServerNameChanged; }
        }
    }
}
