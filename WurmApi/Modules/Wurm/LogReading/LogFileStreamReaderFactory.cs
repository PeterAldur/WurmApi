using System;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogReading
{
    class LogFileStreamReaderFactory
    {
        readonly IWurmApiConfig wurmApiConfig;

        public LogFileStreamReaderFactory([NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (wurmApiConfig == null) throw new ArgumentNullException("wurmApiConfig");
            this.wurmApiConfig = wurmApiConfig;
        }

        public LogFileStreamReader Create(
            string fileFullPath,
            long startPosition = 0,
            bool trackFileBytePositions = false)
        {
            if (wurmApiConfig.Platform == Platform.Windows)
            {
                return new LogFileCrLfStreamReader(fileFullPath, startPosition, trackFileBytePositions);
            }
            else
            {
                return new LogFileLfStreamReader(fileFullPath, startPosition, trackFileBytePositions);
            }
        }
    }
}
