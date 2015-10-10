using System.IO;

namespace AldursLab.WurmApi.Modules.Wurm.Paths
{
    class WurmPaths : IWurmPaths
    {
        private readonly string configsDirPath;
        private readonly string playersDirPath;

        public WurmPaths(IWurmClientInstallDirectory wurmInstallDirectory)
        {
            configsDirPath = Path.Combine(wurmInstallDirectory.FullPath, "configs");
            playersDirPath = Path.Combine(wurmInstallDirectory.FullPath, "players");
        }

        public string ConfigsDirFullPath
        {
            get { return configsDirPath; }
        }

        public string CharactersDirFullPath
        {
            get { return playersDirPath; }
        }

        public string GetSkillDumpsFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath, characterName.Capitalized, "dumps");
        }
    }
}
