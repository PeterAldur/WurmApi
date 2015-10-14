using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Modules.Wurm.ServerGroups
{
    class WurmServerGroups : IWurmServerGroups
    {
        readonly ServerGroup[] groups = new ServerGroup[]
        {
            new UnknownServerGroup(),
            new ChallengeServerGroup(),
            new EpicServerGroup(),
            new FreedomServerGroup()
        };

        public IEnumerable<ServerGroup> All
        {
            get { return groups; }
        }

        public ServerGroup GetById(ServerGroupId serverGroupId)
        {
            var result = All.FirstOrDefault(@group => @group.ServerGroupId == serverGroupId);
            if (result == null)
            {
                throw new DataNotFoundException(
                    "No known ServerGroup for this ServerGroupId. Perhaps Id is outside defined enum values?");
            }
            return result;
        }
    }

    public class UnknownServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Unknown; }
        }

        public override string Description
        {
            get { return "All servers not known to WurmApi"; }
        }
    }

    public class EpicServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Epic; }
        }

        public override string Description
        {
            get { return "Epic servers group including Elevation"; }
        }
    }

    public class FreedomServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Freedom; }
        }

        public override string Description
        {
            get { return "Freedom servers group including Chaos"; }
        }
    }

    public class ChallengeServerGroup : ServerGroup
    {
        public override ServerGroupId ServerGroupId
        {
            get { return ServerGroupId.Challenge; }
        }

        public override string Description
        {
            get { return "Challenge servers group"; }
        }
    }
}