using System;
using System.IO;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Paths
{
    class WurmPaths : IWurmPaths
    {
        readonly IWurmApiConfig wurmApiConfig;

        private readonly string configsDirPath;
        private readonly string playersDirPath;

        public WurmPaths(IWurmClientInstallDirectory wurmInstallDirectory, [NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (wurmApiConfig == null) throw new ArgumentNullException("wurmApiConfig");
            this.wurmApiConfig = wurmApiConfig;
            configsDirPath = Path.Combine(wurmInstallDirectory.FullPath, "configs");
            playersDirPath = Path.Combine(wurmInstallDirectory.FullPath, "players");
        }

        public string LogsDirName
        {
            get { return wurmApiConfig.WurmUnlimitedMode ? "test_logs" : "logs"; }
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
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                wurmApiConfig.WurmUnlimitedMode ? "test_dumps" : "dumps");
        }

        public string GetLogsDirFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                LogsDirName);
        }

        public string GetScreenshotsDirFullPathForCharacter(CharacterName characterName)
        {
            return Path.Combine(playersDirPath,
                characterName.Capitalized,
                wurmApiConfig.WurmUnlimitedMode ? "test_screenshots" : "screenshots");
        }
    }
}
