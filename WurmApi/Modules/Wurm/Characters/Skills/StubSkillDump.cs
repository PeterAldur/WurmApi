using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class StubSkillDump : SkillDump
    {
        public StubSkillDump(ServerGroup serverGroup) : base(serverGroup)
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

        public override IEnumerable<SkillInfo> All
        {
            get { return new SkillInfo[0]; }
        }

        public override bool IsNull
        {
            get { return true; }
        }
    }
}