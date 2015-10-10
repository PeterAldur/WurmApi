using System;

namespace AldursLab.WurmApi.Modules.Wurm.Characters.Skills
{
    class SkillInfo
    {
        public SkillInfo(string name, float value, DateTime stamp)
        {
            NameNormalized = WurmSkills.NormalizeSkillName(name);
            Value = value;
            Stamp = stamp;
        }

        public string NameNormalized { get; private set; }
        public float Value { get; private set; }
        public DateTime Stamp { get; private set; }
    }
}