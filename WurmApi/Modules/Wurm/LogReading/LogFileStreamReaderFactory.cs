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
            // why are there 2 stream readers?
            // wurm has a nasty habbit of adding /n in log lines on windows, 
            // this happens if a newline-separated text is pasted into chat input
            if (wurmApiConfig.Platform == Platform.Windows)
            {
                return new LogFileCrLfStreamReader(fileFullPath, startPosition, trackFileBytePositions);
            }
            else
            {
                if (trackFileBytePositions)
                {
                    throw new NotSupportedException("trackFileBytePositions is not supported outside Windows platform");
                }
                return new LogFileLfStreamReader(fileFullPath, startPosition, false);
            }
        }

        public LogFileStreamReader CreateWithLineCountFastForward(
            string fileFullPath,
            int lineCountToSkip)
        {
            var reader = Create(fileFullPath, 0, false);
            reader.FastForwardLinesCount(lineCountToSkip);
            return reader;
        }
    }
}
